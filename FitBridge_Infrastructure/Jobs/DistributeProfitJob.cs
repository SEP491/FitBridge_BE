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
        var orderItemId = Guid.Parse(context.JobDetail.JobDataMap.GetString("orderItemId")
            ?? throw new NotFoundException($"{nameof(DistributeProfitJob)}_orderItemId"));
        _logger.LogInformation("DistributeProfitJob started for OrderItem: {OrderItemId}", orderItemId);

        await _transactionService.DistributeProfit(orderItemId);
    }
}