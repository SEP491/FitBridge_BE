using System;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Dtos.GymCourses;

namespace FitBridge_Application.Dtos.OrderItems;

public class OrderItemForCourseOrderResponseDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public bool IsFeedback { get; set; }
    public Guid OrderId { get; set; }
    public Guid? GymCourseId { get; set; }
    public Guid? FreelancePTPackageId { get; set; }
    public string ProductName { get; set; }
    public GetFreelancePTPackageWithPt? FreelancePTPackage { get; set; }
    public GymCourseResponse? GymCourse { get; set; }
    public bool IsRefunded { get; set; }
}
