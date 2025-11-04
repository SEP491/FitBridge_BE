using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.SubscriptionPlans;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Subscriptions.CancelSubscription;

public class CancelSubscriptionCommandHandler(IUnitOfWork unitOfWork, IScheduleJobServices _scheduleJobServices) : IRequestHandler<CancelSubscriptionCommand, bool>
{
    public async Task<bool> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userSubscription = await unitOfWork.Repository<UserSubscription>().GetByIdAsync(request.userSubscriptionId);
        if (userSubscription == null)
        {
            throw new NotFoundException("User subscription not found");
        }
        await _scheduleJobServices.CancelScheduleJob($"ExpireUserSubscription_{request.userSubscriptionId}", "ExpireUserSubscription");

        await _scheduleJobServices.CancelScheduleJob($"SendRemindExpiredSubscriptionNoti_{request.userSubscriptionId}", "SendRemindExpiredSubscriptionNoti");

        userSubscription.Status = SubScriptionStatus.Cancelled;
        userSubscription.UpdatedAt = DateTime.UtcNow;
        
        unitOfWork.Repository<UserSubscription>().Update(userSubscription);
        await unitOfWork.CommitAsync();
        return true;
    }

}
