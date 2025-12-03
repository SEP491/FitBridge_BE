using System;
using FitBridge_Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Features.Identities.Registers.ResendEmail;

public class ResendEmailConfirmationCommandHandler(IApplicationUserService _applicationUserService, IEmailService _emailService, IConfiguration _configuration) : IRequestHandler<ResendEmailConfirmationCommand, bool>
{
    public async Task<bool> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _applicationUserService.GetUserByEmailAsync(request.Email, false);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }
        var token = await _applicationUserService.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?token={token}&email={user.Email}";
        await _emailService.SendRegistrationConfirmationEmailAsync(user.Email, confirmationLink, user.FullName);
        return true;
    }
}
