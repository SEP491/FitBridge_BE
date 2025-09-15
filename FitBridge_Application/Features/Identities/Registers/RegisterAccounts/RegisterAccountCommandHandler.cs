using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FitBridge_Application.Features.Identities.Registers.RegisterAccounts;

public class RegisterAccountCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration _configuration, IEmailService emailService) : IRequestHandler<RegisterAccountCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
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
            Password = request.Password,
            GymName = request.GymName ?? "",
            TaxCode = request.TaxCode ?? "",
            EmailConfirmed = true,
        };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new BaseResponse<string>("400", "User creation failed", null);
        }
        switch (request.Role)
        {
            case ProjectConstant.UserRoles.GymOwner:
                await userManager.AddToRoleAsync(user, ProjectConstant.UserRoles.GymOwner);
                await emailService.SendAccountInformationEmailAsync(user.Email, request.Password);

                break;
            case ProjectConstant.UserRoles.GymPT:
                await userManager.AddToRoleAsync(user, ProjectConstant.UserRoles.GymPT);
                await emailService.SendAccountInformationEmailAsync(user.Email, request.Password);
                break;
            case ProjectConstant.UserRoles.FreelancePT:
                await userManager.AddToRoleAsync(user, ProjectConstant.UserRoles.FreelancePT);
                await emailService.SendAccountInformationEmailAsync(user.Email, request.Password);
                break;
            case ProjectConstant.UserRoles.Admin:
                await userManager.AddToRoleAsync(user, ProjectConstant.UserRoles.Admin);
                break;
        }

        return new BaseResponse<string>("200", "User created successfully", user.Id.ToString());
    }
}
