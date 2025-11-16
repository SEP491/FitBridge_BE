using System;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Domain.Enums.Orders;
using MediatR;

namespace FitBridge_Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<OrderStatusResponseDto>
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Description { get; set; }
}
