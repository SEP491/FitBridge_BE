using System;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using AutoMapper;

namespace FitBridge_Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<UpdateOrderStatusCommand, OrderStatusResponseDto>
{
    public async Task<OrderStatusResponseDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId);
        if (order == null)
        {
            throw new NotFoundException("Order not found");
        }
        if (request.Status == OrderStatus.Processing)
        {
            if(order.Status != OrderStatus.Pending)
            {
                throw new WrongStatusSequenceException("Order status is not pending");
            }
        }
        if(request.Status == OrderStatus.Cancelled)
        {
            if(order.Status != OrderStatus.Created || order.Status != OrderStatus.Pending)
            {
                throw new WrongStatusSequenceException("Order status is not created or pending");
            }
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
}
