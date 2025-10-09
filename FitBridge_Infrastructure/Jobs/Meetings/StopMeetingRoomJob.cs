using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Meetings;
using FitBridge_Domain.Entities.Meetings;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Services.Meetings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FitBridge_Infrastructure.Jobs.Meetings
{
    internal class StopMeetingRoomJob(
        IHubContext<SignalingHub, ISignalingClients> hubContext,
        ILogger<StopMeetingRoomJob> logger,
        IUnitOfWork unitOfWork) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var roomId = Guid.Parse(context.JobDetail.JobDataMap.GetString("roomId")
                ?? throw new NotFoundException($"{nameof(StopMeetingRoomJob)}_roomId"));

            await hubContext.Clients.Group(roomId.ToString().ToLower())
                .StopMeeting();
            logger.LogInformation("StopMeetingRoomJob started {Room}", roomId);

            unitOfWork.Repository<MeetingSession>().SoftDelete(roomId);
            await unitOfWork.CommitAsync();
        }
    }
}