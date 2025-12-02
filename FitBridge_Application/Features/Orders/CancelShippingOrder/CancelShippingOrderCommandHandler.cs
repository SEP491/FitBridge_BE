using System;
using MediatR;

using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Orders.CancelShippingOrder;

public class CancelShippingOrderCommandHandler(IUnitOfWork _unitOfWork, IAhamoveService _ahamoveService) : IRequestHandler<CancelShippingOrderCommand, bool>
{
    public async Task<bool> Handle(CancelShippingOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId);
        if (order == null)
        {
            throw new NotFoundException("Order not found");
        }
        await _ahamoveService.CancelShippingOrderAsync(request.OrderId, request.Comment);
        return true;
    }
}
