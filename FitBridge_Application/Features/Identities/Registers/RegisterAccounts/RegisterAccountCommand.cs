using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Identities;
using MediatR;

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
}
