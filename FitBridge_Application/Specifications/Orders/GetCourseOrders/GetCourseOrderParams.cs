using System;

namespace FitBridge_Application.Specifications.Orders.GetCourseOrders;

public class GetCourseOrderParams : BaseParams
{
    public Guid? CustomerId { get; set; }
    public bool IsFreelancePtCourse { get; set; } = false;
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? OrderId { get; set; }
}
