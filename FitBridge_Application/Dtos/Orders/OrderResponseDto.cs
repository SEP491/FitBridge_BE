using System;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Orders;

public class OrderResponseDto
{
    public OrderStatus Status { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal SubTotalPrice { get; set; }
    public Guid? AddressId { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid? CustomerPurchasedIdToExtend { get; set; }
    public Guid? GymCoursePTIdToAssign { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CouponId { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}
