using System;
using FitBridge_Application.Dtos.GymCoursePts;

namespace FitBridge_Application.Dtos.CustomerPurchaseds;

public class CustomerPurchasedResponseDto
{
    public Guid Id { get; set; }

    // Package Information
    public string PackageName { get; set; } = string.Empty;
    public string CourseImageUrl { get; set; } = string.Empty;

    public int AvailableSessions { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public bool CanAssignPT { get; set; }
    public decimal PTAssignmentPrice { get; set; }
    public Guid? GymCourseId { get; set; }
}
