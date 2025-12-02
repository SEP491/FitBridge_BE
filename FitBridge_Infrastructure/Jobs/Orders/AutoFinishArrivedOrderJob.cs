using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FitBridge_Infrastructure.Jobs.Orders;

public class AutoFinishArrivedOrderJob(ILogger<AutoFinishArrivedOrderJob> _logger, IUnitOfWork _unitOfWork) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var orderId = Guid.Parse(context.JobDetail.JobDataMap.GetString("orderId")
            ?? throw new NotFoundException($"{nameof(AutoFinishArrivedOrderJob)}_orderId"));
        _logger.LogInformation("AutoFinishArrivedOrderJob started for Order: {OrderId}", orderId);
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogError("Order not found for OrderId: {OrderId}", orderId);
            return;
        }
        order.Status = OrderStatus.Finished;
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CommitAsync();
    }

}
