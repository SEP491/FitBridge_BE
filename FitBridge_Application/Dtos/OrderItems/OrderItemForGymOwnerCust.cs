using System;

namespace FitBridge_Application.Dtos.OrderItems;

public class OrderItemForGymOwnerCust
{
    public Guid OrderItemId { get; set; }
    public Guid? GymCourseId { get; set; }
    public Guid? GymPtId { get; set; }
    public string? CourseName { get; set; }
    public string? PtName { get; set; }
    public string? PtImageUrl { get; set; }
    public decimal? AmountSpend { get; set; }
    public Guid? CustomerPurchasedId { get; set; }
}
