using System;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Orders;

public class CourseOrderResponseDto
{
    public Guid Id { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal SubTotalPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? CouponId { get; set; }
    public double DiscountPercent { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<OrderItemForCourseOrderResponseDto> OrderItems { get; set; } = new List<OrderItemForCourseOrderResponseDto>();
}
