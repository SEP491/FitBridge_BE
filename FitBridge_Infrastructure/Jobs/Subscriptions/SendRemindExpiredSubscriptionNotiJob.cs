using System;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.ServicePackages;
using Quartz;
using FitBridge_Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Jobs.Subscriptions;

public class SendRemindExpiredSubscriptionNotiJob(IUnitOfWork _unitOfWork, ILogger<SendRemindExpiredSubscriptionNotiJob> _logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var userSubscriptionId = Guid.Parse(context.JobDetail.JobDataMap.GetString("userSubscriptionId") ?? throw new NotFoundException($"{nameof(SendRemindExpiredSubscriptionNotiJob)}_userSubscriptionId"));
        var userSubscription = await _unitOfWork.Repository<UserSubscription>().GetByIdAsync(userSubscriptionId);
        if (userSubscription == null)
        {
            _logger.LogError($"User subscription not found for user subscription id {userSubscriptionId}");
            return;
        }
        //Todo: Send remind expired subscription notification to user
    }
}
