using System;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.ServicePackages;
using Quartz;
using FitBridge_Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using FitBridge_Domain.Enums.SubscriptionPlans;
using FitBridge_Application.Services;

namespace FitBridge_Infrastructure.Jobs.Subscriptions;

public class ExpireUserSubscriptionJob(IUnitOfWork _unitOfWork, ILogger<ExpireUserSubscriptionJob> _logger, SubscriptionService subscriptionService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var userSubscriptionId = Guid.Parse(context.JobDetail.JobDataMap.GetString("userSubscriptionId") ?? throw new NotFoundException($"{nameof(ExpireUserSubscriptionJob)}_ExpireUserSubscription"));

        await subscriptionService.ExpireUserSubscription(userSubscriptionId);
        
        _logger.LogInformation($"Successfully expired user subscription {userSubscriptionId}");
    }
}
