using System;
using FitBridge_Application.Dtos.Accounts.UserDetails;

namespace FitBridge_Application.Dtos.Accounts.Profiles;

public class UpdateProfileResponseDto
{
    public Guid? Id { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? Dob { get; set; }
    public bool? IsMale { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? TaxCode { get; set; }
    public string? GymDescription { get; set; }
    public string? GymName { get; set; }
    public DateOnly? GymFoundationDate { get; set; }
    public DateOnly? IdentityCardDate { get; set; }
    public string? BusinessAddress { get; set; }
    public string? Bio { get; set; }
    public List<string> FreelancePtImages { get; set; } = new List<string>();
    public List<string> GymImages { get; set; } = new List<string>();
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public UpdateUserDetailDto? UserDetail { get; set; }
}
