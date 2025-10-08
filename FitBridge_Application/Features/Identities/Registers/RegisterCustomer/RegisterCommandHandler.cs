using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Dtos.Emails;

namespace FitBridge_Application.Features.Identities.Registers.RegisterCustomer;

public class RegisterCommandHandler(IApplicationUserService _applicationUserService, IEmailService emailService, IConfiguration _configuration, IUnitOfWork _unitOfWork) : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Dob = request.Dob,
            IsMale = request.IsMale,
            Password = request.Password,
            EmailConfirmed = request.IsTestAccount,
        };
        await _applicationUserService.InsertUserAsync(user, request.Password);
        await _applicationUserService.AssignRoleAsync(user, ProjectConstant.UserRoles.Customer);

        var userDetail = new UserDetail { Id = user.Id };
        _unitOfWork.Repository<UserDetail>().Insert(userDetail);
        await _unitOfWork.CommitAsync();

        var token = await _applicationUserService.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?token={token}&email={user.Email}";

        if (!request.IsTestAccount)
        {
            await SendConfirmationEmail(user, confirmationLink);
        }
        return new RegisterResponseDto { UserId = user.Id };
    }

    private async Task SendConfirmationEmail(ApplicationUser user, string confirmationLink)
    {
        var emailData = new AccountInformationEmailData
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Dob = user.Dob,
            IsMale = user.IsMale,
            ConfirmationLink = confirmationLink,
            EmailType = ProjectConstant.EmailTypes.RegistrationConfirmationEmail
        };
        await emailService.ScheduleEmailAsync(emailData);
    }
}
