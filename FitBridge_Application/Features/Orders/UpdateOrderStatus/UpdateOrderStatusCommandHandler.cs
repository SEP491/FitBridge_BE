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

namespace FitBridge_Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IScheduleJobServices _scheduleJobServices) : IRequestHandler<UpdateOrderStatusCommand, OrderStatusResponseDto>
{
    public async Task<OrderStatusResponseDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, includes: new List<string> { nameof(Order.OrderItems), "OrderItems.ProductDetail", "Transactions" });
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
            if (order.Status != OrderStatus.Created && order.Status != OrderStatus.Pending)
            {
                throw new WrongStatusSequenceException("Order status is not created or pending");
            }
            if (paymentMethod.MethodType != MethodType.COD)
            {
                throw new BusinessException("Payment method is not COD, cannot cancel order");
            }
            if (order.Status == OrderStatus.Pending)
            {
                await ReturnQuantityToProductDetail(order);
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
        _unitOfWork.Repository<Order>().Update(order);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<OrderStatusResponseDto>(orderStatusHistory);
    }
    
    public async Task<bool> ReturnQuantityToProductDetail(Order order)
    {
        foreach (var orderItem in order.OrderItems)
        {
            var productDetail = await _unitOfWork.Repository<ProductDetail>().GetByIdAsync(orderItem.ProductDetailId.Value);
            if (productDetail == null)
            {
                throw new NotFoundException("Product detail not found");
            }
            productDetail.Quantity += orderItem.Quantity;
            productDetail.SoldQuantity -= orderItem.Quantity;
            _unitOfWork.Repository<ProductDetail>().Update(productDetail);
        }
        return true;
    }
}
