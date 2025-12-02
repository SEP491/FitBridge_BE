using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FitBridge_Infrastructure.Jobs.Reviews;

public class MarkAsFeedbackJob(ILogger<MarkAsFeedbackJob> _logger, IUnitOfWork _unitOfWork) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var orderItemId = Guid.Parse(context.JobDetail.JobDataMap.GetString("orderItemId")
            ?? throw new NotFoundException($"{nameof(MarkAsFeedbackJob)}_orderItemId"));
        _logger.LogInformation("MarkAsFeedbackJob started for OrderItem: {OrderItemId}", orderItemId);
        var orderItem = await _unitOfWork.Repository<OrderItem>().GetByIdAsync(orderItemId);
        if (orderItem == null)
        {
            _logger.LogError("OrderItem not found for OrderItemId: {OrderItemId}", orderItemId);
            throw new NotFoundException($"{nameof(MarkAsFeedbackJob)}_orderItemId");
        }
        orderItem.IsFeedback = true;
        _unitOfWork.Repository<OrderItem>().Update(orderItem);
        await _unitOfWork.CommitAsync();
    }
}
