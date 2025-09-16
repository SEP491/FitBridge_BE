using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Identities;
using MediatR;

namespace FitBridge_Application.Features.Identities.Registers;

public class RegisterCommand : IRequest<RegisterResponseDto>
{
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public string Password { get; set; }
    public string FullName { get; set; }
    public DateTime Dob { get; set; }
    public bool IsMale { get; set; }
    public bool IsTestAccount { get; set; } = false;
}
