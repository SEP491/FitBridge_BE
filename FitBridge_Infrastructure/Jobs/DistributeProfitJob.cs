using System;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FitBridge_Infrastructure.Jobs;

public class DistributeProfitJob(IUnitOfWork _unitOfWork, ILogger<DistributeProfitJob> _logger, ITransactionService _transactionService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var customerPurchasedId = Guid.Parse(context.JobDetail.JobDataMap.GetString("customerPurchasedId")
            ?? throw new NotFoundException($"{nameof(DistributeProfitJob)}_customerPurchasedId"));
        _logger.LogInformation("DistributeProfitJob started for CustomerPurchased: {CustomerPurchasedId}", customerPurchasedId);

        await _transactionService.DistributeProfit(customerPurchasedId);
    }
}