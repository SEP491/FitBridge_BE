using System;

namespace FitBridge_Application.Dtos.FreelancePTPackages;

public class GetCustomerPurchasedForFreelancePt
{
    public Guid Id { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public string CourseImageUrl { get; set; } = string.Empty;
    public int AvailableSessions { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public Guid? FreelancePTPackageId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerImageUrl { get; set; } = string.Empty;
}
