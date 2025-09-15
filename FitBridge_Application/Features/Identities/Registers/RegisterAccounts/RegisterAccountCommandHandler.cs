using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FitBridge_Application.Features.Identities.Registers.RegisterAccounts;

public class RegisterAccountCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration _configuration, IEmailService emailService) : IRequestHandler<RegisterAccountCommand, RegisterResponseDto>
{
    public async Task<RegisterResponseDto> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        var isEmailExists = await userManager.FindByEmailAsync(request.Email);
        if (isEmailExists is not null)
        {
            return new RegisterResponseDto { Status = "400", Message = "Email already exists", UserId = Guid.Empty };
        }

        var isPhoneNumberExists = await userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber);
        if (isPhoneNumberExists)
        {
            return new RegisterResponseDto { Status = "400", Message = "Phone number already exists", UserId = Guid.Empty };
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
            return new RegisterResponseDto { Status = "400", Message = "User creation failed", UserId = Guid.Empty };
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

        return new RegisterResponseDto { Status = "200", Message = "User created successfully", UserId = user.Id };
    }
}
