using System;
using MediatR;
using FitBridge_Application.Dtos.Identities;

namespace FitBridge_Application.Features.Identities.Login;

public class LoginUserCommand : IRequest<LoginResponseDTO>
{
    public string Identifier { get; set; } 
    public string Password { get; set; }
}