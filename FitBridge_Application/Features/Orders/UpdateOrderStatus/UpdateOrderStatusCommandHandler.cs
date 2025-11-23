using System;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using AutoMapper;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Services;

namespace FitBridge_Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IScheduleJobServices _scheduleJobServices, OrderService _orderService) : IRequestHandler<UpdateOrderStatusCommand, OrderStatusResponseDto>
{
    public async Task<OrderStatusResponseDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId,false, includes: new List<string> { nameof(Order.OrderItems), "OrderItems.ProductDetail", "Transactions" });
        var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(order.Transactions.FirstOrDefault(t => t.TransactionType == TransactionType.ProductOrder)!.PaymentMethodId);
        if (order == null)
        {
            throw new NotFoundException("Order not found");
        }
        if (request.Status == OrderStatus.Processing)
        {
            if (order.Status != OrderStatus.Pending)
            {
                throw new WrongStatusSequenceException("Order status is not pending");
            }
        }
        if (request.Status == OrderStatus.Cancelled)
        {
            if (order.Status != OrderStatus.Created && order.Status != OrderStatus.Pending && order.Status != OrderStatus.Returned)
            {
                throw new WrongStatusSequenceException("Order status is not created or pending or returned");
            }
            if (paymentMethod.MethodType != MethodType.COD && order.Status != OrderStatus.Returned)
            {
                throw new BusinessException("Payment method is not COD, cannot cancel order");
            }
            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Returned)
            {
                await _orderService.ReturnQuantityToProductDetail(order);
            }

        }
        if (request.Status == OrderStatus.CustomerNotReceived)
        {
            if (order.Status != OrderStatus.Arrived)
            {
                throw new WrongStatusSequenceException("Order status is not arrived");
            }
            await _scheduleJobServices.CancelScheduleJob($"AutoFinishArrivedOrder_{order.Id}", "AutoFinishArrivedOrder");
        }
        var previousStatus = order.Status;
        order.Status = request.Status;
        var orderStatusHistory = new OrderStatusHistory
        {
            OrderId = order.Id,
            Status = request.Status,
            Description = request.Description,
            PreviousStatus = previousStatus,
        };
        _unitOfWork.Repository<OrderStatusHistory>().Insert(orderStatusHistory);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<OrderStatusResponseDto>(orderStatusHistory);
    }

}
