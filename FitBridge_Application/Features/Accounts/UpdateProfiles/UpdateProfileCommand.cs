using System;
using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.Accounts.Profiles;
using FitBridge_Application.Dtos.Accounts.UserDetails;
using MediatR;

namespace FitBridge_Application.Features.Accounts.UpdateProfiles;

public class UpdateProfileCommand : IRequest<UpdateProfileResponseDto>
{
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool? IsMale { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public DateTime? Dob { get; set; }
    public string? TaxCode { get; set; }
    public string? GymDescription { get; set; }
    public string? GymName { get; set; }
    public string? IdentityCardPlace { get; set; }
    public string? CitizenCardPermanentAddress { get; set; }
    public string? CitizenIdNumber { get; set; }
    public UpdateUserDetailDto? UserDetail { get; set; }
}
