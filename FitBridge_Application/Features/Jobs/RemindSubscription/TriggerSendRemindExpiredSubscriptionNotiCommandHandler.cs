using System;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FitBridge_Application.Features.Jobs.RemindSubscription;

public class TriggerSendRemindExpiredSubscriptionNotiCommandHandler(IScheduleJobServices _scheduleJobServices) : IRequestHandler<TriggerSendRemindExpiredSubscriptionNotiCommand, bool>   
{
    public async Task<bool> Handle(TriggerSendRemindExpiredSubscriptionNotiCommand request, CancellationToken cancellationToken) 
    {
        return await _scheduleJobServices.RescheduleJob($"SendRemindExpiredSubscriptionNoti_{request.UserSubscriptionId}", "SendRemindExpiredSubscriptionNoti", DateTime.UtcNow);
    }
}
