using System;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Application.Interfaces.Repositories;
using Quartz;
using Microsoft.Extensions.Logging;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Infrastructure.Jobs.Orders;

public class CancelCreatedOrderJob(ILogger<CancelCreatedOrderJob> _logger, IUnitOfWork _unitOfWork, OrderService orderService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var orderId = Guid.Parse(context.JobDetail.JobDataMap.GetString("orderId")
            ?? throw new NotFoundException($"{nameof(CancelCreatedOrderJob)}_orderId"));
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId, includes: new List<string> { nameof(Order.OrderItems), "Transactions", "OrderItems.GymCourse", "OrderItems.FreelancePTPackage", "OrderItems.UserSubscription" });
        if (order == null)
        {
            throw new NotFoundException("Order not found");
        }
        if (order.Status != OrderStatus.Created)
        {
            _logger.LogError("Order is not created, current status: {OrderStatus}", order.Status);
            return;
        }
        if (order.Transactions.Any(t => t.TransactionType == TransactionType.SubscriptionPlansOrder))
        {
            _unitOfWork.Repository<UserSubscription>().Delete(order.OrderItems.First().UserSubscription);
            await _unitOfWork.CommitAsync();
            return;
        }
        if (order.Transactions.Any(t => t.TransactionType == TransactionType.ProductOrder))
        {
            await orderService.ReturnQuantityToProductDetail(order);
        }
        if(order.Transactions.Any(t => t.TransactionType == TransactionType.GymCourse || t.TransactionType == TransactionType.FreelancePTPackage))
        {
            await orderService.ReturnQuantityToPT(order);
        }
        order.Status = OrderStatus.Cancelled;
        var statusHistory = new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = OrderStatus.Cancelled,
            Description = "Order cancelled by system",
            PreviousStatus = OrderStatus.Created,
        };
        _unitOfWork.Repository<OrderStatusHistory>().Insert(statusHistory);
        var transactionToUpdate = order.Transactions.FirstOrDefault(t => t.Status == TransactionStatus.Pending);
        if(transactionToUpdate != null)
        {
            transactionToUpdate.Status = TransactionStatus.Failed;
            transactionToUpdate.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Transaction>().Update(transactionToUpdate);
        }
        order.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CommitAsync();
    }
}
