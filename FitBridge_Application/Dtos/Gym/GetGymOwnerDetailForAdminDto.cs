using System;

namespace FitBridge_Application.Dtos.Gym;

public class GetGymOwnerDetailForAdminDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime Dob { get; set; }
    public string? GymName { get; set; }
    public string? TaxCode { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public bool hotResearch { get; set; }
    public string? GymDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AvatarUrl { get; set; }
    public int MinimumSlot { get; set; } // Minimum slot register perweek, control by gym owner account
    public bool IsActive { get; set; }
    public List<string> GymImages { get; set; } = new List<string>();
}
