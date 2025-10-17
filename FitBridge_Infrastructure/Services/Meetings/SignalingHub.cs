using FitBridge_Application.Configurations;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Meetings;
using FitBridge_Domain.Entities.Meetings;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Jobs.Meetings;
using FitBridge_Infrastructure.Services.Meetings.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FitBridge_Infrastructure.Services.Meetings
{
    [Authorize]
    public class SignalingHub(
        ILogger<SignalingHub> logger,
        SessionManager sessionManager,
        IOptions<MeetingSettings> options,
        ISchedulerFactory schedulerFactory,
        IUnitOfWork unitOfWork) : Hub<ISignalingClients>
    {
        private readonly JsonSerializerOptions jsonSerializerOptions =
        new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>

        /// Adds a user to a WebRTC room and notifies other participants
        /// </summary>
        /// <param name="roomId">The ID of the room to join</param>
        /// <returns>A bool task representing whether the callee is a polite peer or not</returns>
        public async Task<bool?> JoinRoom(string strRoomId)
        {
            var roomId = await TryParseGuid(strRoomId);
            if (roomId == Guid.Empty)
            {
                return null;
            }
            var userId = Context.UserIdentifier;
            var connectionId = Context.ConnectionId;
            var meetingSession = await unitOfWork.Repository<MeetingSession>()
                .GetByIdAsync(roomId, includes: [nameof(Booking)]);

            if (meetingSession == null)
            {
                logger.LogInformation("Room {RoomId} does not exist", roomId);
                var error = new HubError
                {
                    Code = HubErrorEnum.ROOM_NOT_FOUND,
                    Message = $"Room {strRoomId} does not exist"
                };
                throw new HubException(JsonSerializer.Serialize(error, jsonSerializerOptions));
            }

            if (meetingSession.UserOneId.ToString() != userId && meetingSession.UserTwoId.ToString() != userId)
            {
                logger.LogInformation("User {UserId} is not authorized to join room {RoomId}", userId, roomId);
                logger.LogInformation("Room {RoomId} does not exist", roomId);
                var error = new HubError
                {
                    Code = HubErrorEnum.NOT_AUTHORIZED_TO_JOIN,
                    Message = $"Room {strRoomId} does not exist"
                };
                throw new HubException(JsonSerializer.Serialize(error, jsonSerializerOptions));
            }

            // if user is the first to join, create a new session
            // if user is the second to join, add them to the session
            var session = await sessionManager.GetCallInfoAsync(strRoomId);
            if (session != null)
            {
                await Groups.AddToGroupAsync(connectionId, strRoomId);
                await sessionManager.AddConnectionToRoomAsync(strRoomId, connectionId);
                await StartSessionCleanupJobAsync(roomId);
                return false; // is impolite peer
            }
            else
            {
                await sessionManager.SetCallInfoAsync(strRoomId, new CallInfo
                {
                    CallDetails = new Dictionary<string, object>
                    {
                        { "StartTime", meetingSession.Booking.PtFreelanceStartTime! }
                    }
                });
                await Groups.AddToGroupAsync(connectionId, strRoomId);
                await sessionManager.AddConnectionToRoomAsync(strRoomId, connectionId);
                return true; // is polite peer
            }
        }

        /// <summary>
        /// Removes a user from a WebRTC room and notifies other participants
        /// </summary>
        /// <param name="roomId">The ID of the room to leave</param>
        /// <param name="username">The username of the user leaving the room</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task LeaveRoom(string strRoomId)
        {
            var roomId = await TryParseGuid(strRoomId);
            if (roomId == Guid.Empty)
            {
                return;
            }
            var connectionId = Context.ConnectionId;

            logger.LogInformation("Removing connection {ConnectionId} from group {RoomId}", connectionId, roomId);

            await Groups.RemoveFromGroupAsync(connectionId, strRoomId);

            logger.LogInformation("Removing connection {ConnectionId} from room {RoomId}", connectionId, roomId);
            var meetingSession = await sessionManager.GetCallInfoAsync(strRoomId);
            if (meetingSession == null)
            {
                logger.LogInformation("No such room exists {RoomId}", roomId);
                return;
            }

            await sessionManager.RemoveConnectionFromRoomAsync(strRoomId, connectionId);
            if (meetingSession.ConnectedConnectionIds.Count == 0)
            {
                logger.LogInformation("No participants left in room {RoomId}, removing session", roomId);
                await sessionManager.RemoveCallInfoAsync(strRoomId);
                unitOfWork.Repository<MeetingSession>().SoftDelete(roomId);
                await KillJobsAsync(roomId);
            }
            else
            {
                logger.LogInformation("Notifying other users in room {RoomId} that connection {ConnectionId} has left", roomId, connectionId);
                var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                await Clients.OthersInGroup(strRoomId).UserLeft(username);
            }
        }

        /// <summary>
        /// Sends a message (offer/answer) to all other users in a room
        /// </summary>
        /// <param name="roomId">The ID of the room</param>
        /// <param name="offer">The SDP message to send</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task SendMessage(string strRoomId, object message)
        {
            var callInfo = await sessionManager.GetCallInfoAsync(strRoomId);
            var isConnectionInRoom = callInfo?.ConnectedConnectionIds.Contains(Context.ConnectionId);
            if (isConnectionInRoom != true)
            {
                logger.LogInformation("Connection {ConnectionId} is not in room {RoomId}, cannot send message", Context.ConnectionId, strRoomId);
                return;
            }
            var roomId = await TryParseGuid(strRoomId);
            if (roomId == Guid.Empty)
            {
                return;
            }
            logger.LogInformation("Sending offer to {RoomId}", roomId);

            await Clients.OthersInGroup(strRoomId).ReceiveMessage(message);
        }

        /// <summary>
        /// Sends an ICE candidate to a specific user
        /// </summary>
        /// <param name="strRoomId">The room ID</param>
        /// <param name="iceCandidate">The ICE candidate to send</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task SendIceCandidate(string strRoomId, object candidate)
        {
            logger.LogInformation("Sending ice candidate ");

            await Clients.OthersInGroup(strRoomId).ReceiveICECandidate(candidate);
        }

        /// <summary>
        /// Handles client disconnection by removing the user from all rooms
        /// </summary>
        /// <param name="exception">The exception that caused the disconnection, if any</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            logger.LogInformation("Connection disconnected: {ConnectionId}", connectionId);

            var allRooms = await sessionManager.GetAllCallInfoAsync();
            foreach (var room in allRooms.Where(r => r.Value.ConnectedConnectionIds.Contains(connectionId)))
            {
                await LeaveRoom(room.Key);
            }
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<Guid> TryParseGuid(string strGuid)
        {
            var roomId = Guid.TryParse(strGuid, out var guid) ? guid : Guid.Empty;

            if (roomId == Guid.Empty)
            {
                logger.LogInformation("Invalid room ID: {RoomId}", strGuid);
                await Clients.Caller.RoomDoesNotExist(strGuid);
                return Guid.Empty;
            }
            return roomId;
        }

        private async Task KillJobsAsync(Guid roomId)
        {
            logger.LogInformation("Killing jobs for room {RoomId}", roomId);
            var scheduler = await schedulerFactory.GetScheduler();
            var stopMeetingJobKey = new JobKey($"StopMeetingRoom_{roomId}", "StopMeetingRoom");
            var stopMeetingTriggerKey = new TriggerKey($"StopMeetingRoomTrigger_{roomId}", "StopMeetingRoomTrigger");
            var alertJobKey = new JobKey($"MeetingRoomExpirationAlert_{roomId}", "MeetingRoomExpirationAlert");
            var alertTriggerKey = new TriggerKey($"MeetingRoomExpirationAlertTrigger_{roomId}", "MeetingRoomExpirationAlertTrigger");
            await scheduler.DeleteJob(stopMeetingJobKey);
            await scheduler.UnscheduleJob(stopMeetingTriggerKey);
            await scheduler.DeleteJob(alertJobKey);
            await scheduler.UnscheduleJob(alertTriggerKey);
        }

        private async Task StartSessionCleanupJobAsync(Guid roomId)
        {
            var scheduler = await schedulerFactory.GetScheduler();
            logger.LogInformation("Starting jobs");

            // StopMeetingRoomJob
            var stopMeetingJobKey = new JobKey(nameof(StopMeetingRoomJob), nameof(StopMeetingRoomJob));
            var stopMeetingTriggerKey = new TriggerKey($"{nameof(StopMeetingRoomJob)}Trigger", nameof(StopMeetingRoomJob));

            var stopMeetingJob = JobBuilder.Create<StopMeetingRoomJob>()
                .WithIdentity(stopMeetingJobKey)
                .WithDescription("Stop meeting room job")
                .UsingJobData("roomId", roomId.ToString())
                .Build();

            var stopMeetingTrigger = TriggerBuilder.Create()
                .WithIdentity(stopMeetingTriggerKey)
                .WithDescription("Stop meeting room trigger")
                .StartAt(DateTime.UtcNow.AddSeconds(options.Value.StopMeetingTime))
                .Build();

            // MeetingRoomExpirationAlertJob
            var alertJobKey = new JobKey(nameof(MeetingRoomExpirationAlertJob), nameof(MeetingRoomExpirationAlertJob));
            var alertTriggerKey = new TriggerKey($"{nameof(MeetingRoomExpirationAlertJob)}Trigger", nameof(MeetingRoomExpirationAlertJob));

            var showMeetingAlertJob = JobBuilder.Create<MeetingRoomExpirationAlertJob>()
                .WithIdentity(alertJobKey)
                .WithDescription("Show meeting room expiration alert job")
                .UsingJobData("roomId", roomId.ToString())
                .Build();

            var showMeetingAlertTrigger = TriggerBuilder.Create()
                .WithIdentity(alertTriggerKey)
                .WithDescription("Show meeting room alert trigger")
                .StartAt(DateTime.UtcNow.AddSeconds(options.Value.ShowMeetingAlertTime))
                .Build();

            await scheduler.ScheduleJob(stopMeetingJob, stopMeetingTrigger);
            await scheduler.ScheduleJob(showMeetingAlertJob, showMeetingAlertTrigger);
        }
    }
}