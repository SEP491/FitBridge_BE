using System;
using MediatR;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Services;

namespace FitBridge_Application.Features.Jobs.ExpiredSubscription;

public class TriggerExpireUserSubscriptionCommandHandler(IScheduleJobServices _scheduleJobServices) : IRequestHandler<TriggerExpireUserSubscriptionCommand, bool>
{
    public async Task<bool> Handle(TriggerExpireUserSubscriptionCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleJobServices.RescheduleJob($"ExpireUserSubscription_{request.UserSubscriptionId}", "ExpireUserSubscription", DateTime.UtcNow);
    }
}
