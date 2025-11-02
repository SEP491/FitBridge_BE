using System;

namespace FitBridge_Application.Dtos.Gym;

public class GetAllGymOwnerForAdminDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime Dob { get; set; }
    public string? GymName { get; set; }
    public string? TaxCode { get; set; }
    public bool hotResearch { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
}
