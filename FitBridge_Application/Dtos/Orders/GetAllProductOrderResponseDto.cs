using System;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Orders;

public class GetAllProductOrderResponseDto
{
    public SummaryProductOrderDto SummaryProductOrder { get; set; }
    public Guid Id { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal? ShippingFeeActualCost { get; set; }
    public string? ShippingTrackingId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public OrderStatus CurrentStatus { get; set; }
    public Guid? CouponId { get; set; }
    public AddressResponseDto ShippingDetail { get; set; }
    public ICollection<OrderItemForProductOrderResponseDto> OrderItems { get; set; } = new List<OrderItemForProductOrderResponseDto>();
}
