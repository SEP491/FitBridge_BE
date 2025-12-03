using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Addresses;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Application.Features.Addresses.CreateAddress;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Identities.Registers.RegisterAccounts;

public class RegisterAccountCommand : IRequest<RegisterResponseDto>
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string? GymName { get; set; }
    public string? TaxCode { get; set; }
    public string Role { get; set; }
    public bool IsTestAccount { get; set; } = false;
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public IFormFile? FrontCitizenIdFile { get; set; }
    public IFormFile? BackCitizenIdFile { get; set; }
    public string? CitizenIdNumber { get; set; }
    public string? IdentityCardPlace { get; set; }
    public string? CitizenCardPermanentAddress { get; set; }
    public DateOnly? IdentityCardDate { get; set; }
    public string? BusinessAddress { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
}
