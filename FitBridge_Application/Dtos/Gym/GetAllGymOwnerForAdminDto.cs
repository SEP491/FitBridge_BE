using System;
using FitBridge_Application.Dtos.GymAssets;

namespace FitBridge_Application.Dtos.Gym;

public class GetAllGymOwnerForAdminDto
{
    public Guid Id { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string? GymName { get; set; }
    public string? TaxCode { get; set; }
    public DateOnly? GymFoundationDate { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool hotResearch { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? GymDescription { get; set; }
    public string? IdentityCardPlace { get; set; }
    public string? CitizenCardPermanentAddress { get; set; }
    public string? CitizenIdNumber { get; set; }
    public DateOnly? IdentityCardDate { get; set; }
    public string? BusinessAddress { get; set; }
    public string? FrontCitizenIdUrl { get; set; }
    public string? BackCitizenIdUrl { get; set; }
    public List<string> GymImages { get; set; } = new List<string>();
    public List<GymAssetResponseDto> GymAssets { get; set; } = [];
}
