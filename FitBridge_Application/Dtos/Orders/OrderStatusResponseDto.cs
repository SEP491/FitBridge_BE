using System;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Orders;

public class OrderStatusResponseDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public OrderStatus? PreviousStatus { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
