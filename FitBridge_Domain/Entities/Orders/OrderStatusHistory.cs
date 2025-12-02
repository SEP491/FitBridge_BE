using System;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Domain.Entities.Orders;

public class OrderStatusHistory : BaseEntity
{
    public Guid OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Description { get; set; }
    public OrderStatus? PreviousStatus { get; set; }
    public Order Order { get; set; }
}
