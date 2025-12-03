using System;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Features.Identities.Registers.ResendEmail;

public class ResendEmailConfirmationCommand : IRequest<bool>
{
    [Required]
    public string Email { get; set; }
}
