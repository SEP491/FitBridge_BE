using System;
using MediatR;

namespace FitBridge_Application.Features.Accounts.UpdatePassword;

public class UpdatePasswordCommand : IRequest<bool>
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
