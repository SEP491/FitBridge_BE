using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FitBridge_Application.Features.Identities.Registers.RegisterCustomer;

public class RegisterCommandHandler(UserManager<ApplicationUser> userManager, IEmailService emailService, IConfiguration _configuration) : IRequestHandler<RegisterCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var isEmailExists = await userManager.FindByEmailAsync(request.Email);
        if (isEmailExists is not null)
        {
            return new BaseResponse<string>("400", "Email already exists", null);
        }

        var isPhoneNumberExists = await userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber);
        if (isPhoneNumberExists)
        {
            return new BaseResponse<string>("400", "Phone number already exists", null);
        }

        var user = new ApplicationUser
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Dob = request.Dob,
            IsMale = request.IsMale,
            Password = request.Password
        };
        var result = await userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return new BaseResponse<string>("400", "User creation failed", null);
        }
        await userManager.AddToRoleAsync(user, ProjectConstant.UserRoles.Customer);
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?token={token}&email={user.Email}";
        await emailService.SendRegistrationConfirmationEmailAsync(user.Email, confirmationLink, user.FullName);
        return new BaseResponse<string>("200", "User created successfully", user.Id.ToString());
    }
}
