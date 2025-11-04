using System;

namespace FitBridge_Application.Dtos.Accounts.Customers;

public class GetAllCustomersForAdminDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public bool IsMale { get; set; }
    public DateTime Dob { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
}
