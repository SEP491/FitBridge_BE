using System;
using MediatR;

namespace FitBridge_Application.Features.Accounts.UpdateLoginInfo;

public class UpdateLoginInfoCommand : IRequest<bool>
{
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
