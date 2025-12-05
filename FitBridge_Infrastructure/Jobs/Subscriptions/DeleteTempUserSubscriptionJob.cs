using System;
using Quartz;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.ServicePackages;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Jobs.Subscriptions;

public class DeleteTempUserSubscriptionJob(IUnitOfWork _unitOfWork, ILogger<DeleteTempUserSubscriptionJob> _logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var userSubscriptionId = Guid.Parse(context.JobDetail.JobDataMap.GetString("userSubscriptionId") ?? throw new NotFoundException($"{nameof(DeleteTempUserSubscriptionJob)}_userSubscriptionId"));
        var userSubscription = await _unitOfWork.Repository<UserSubscription>().GetByIdAsync(userSubscriptionId);
        if (userSubscription == null)
        {
            _logger.LogError($"User subscription not found for user subscription id {userSubscriptionId}");
        }
        _unitOfWork.Repository<UserSubscription>().Delete(userSubscription);
        await _unitOfWork.CommitAsync();
    }
}
