using System;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Domain.Enums.Orders;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommand : IRequest<OrderStatusResponseDto>
{
    [JsonIgnore]
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Description { get; set; }
}
