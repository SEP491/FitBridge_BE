using System;

namespace FitBridge_Application.Dtos.CustomerPurchaseds;

public class CustomerPurchasedFreelancePtResponseDto
{
    public Guid Id { get; set; }

    public string PackageName { get; set; } = string.Empty;

    public string CourseImageUrl { get; set; } = string.Empty;

    public int AvailableSessions { get; set; }

    public DateOnly ExpirationDate { get; set; }

    public Guid? FreelancePTPackageId { get; set; }
}