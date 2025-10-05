using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Meetings;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Services.Meetings;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FitBridge_Infrastructure.Jobs
{
    internal class MeetingRoomExpirationAlertJob(
        IHubContext<SignalingHub, ISignalingClients> hubContext,
        ILogger<MeetingRoomExpirationAlertJob> logger) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var roomId = Guid.Parse(context.JobDetail.JobDataMap.GetString("roomId")
                ?? throw new NotFoundException($"{nameof(MeetingRoomExpirationAlertJob)}_roomId"));
            logger.LogInformation("MeetingRoomExpirationAlertJob started {Room}", roomId.ToString());
            await hubContext.Clients.Group(roomId.ToString().ToLower())
                .ShowExpirationAlert();
        }
    }
}