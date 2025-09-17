using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Identities;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace FitBridge_Application.Features.Identities.Registers.RegisterAccounts;

public class RegisterAccountCommandHandler(IApplicationUserService _applicationUserService, IConfiguration _configuration, IEmailService emailService, IUnitOfWork _unitOfWork) : IRequestHandler<RegisterAccountCommand, RegisterResponseDto>
{
    public async Task<RegisterResponseDto> Handle(RegisterAccountCommand request, CancellationToken cancellationToken)
    {
        if (request.Role != ProjectConstant.UserRoles.Admin
        && request.Role != ProjectConstant.UserRoles.FreelancePT
        && request.Role != ProjectConstant.UserRoles.GymOwner)
        {
            throw new Exception("Role not found, only Admin, FreelancePT and GymOwner are allowed to register");
        }
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Dob = request.Dob,
            IsMale = request.IsMale,
            Password = request.Password,
            GymName = request.GymName ?? "",
            TaxCode = request.TaxCode ?? "",
            EmailConfirmed = request.IsTestAccount,
        };
        await _applicationUserService.InsertUserAsync(user, request.Password);

        switch (request.Role)
        {
            case ProjectConstant.UserRoles.GymOwner:
                await _applicationUserService.AssignRoleAsync(user, ProjectConstant.UserRoles.GymOwner);
                await SendAccountInformationEmail(user, request.Password, request.IsTestAccount, ProjectConstant.UserRoles.GymOwner);
                await InsertUserDetail(user);
                break;
            case ProjectConstant.UserRoles.FreelancePT:
                await _applicationUserService.AssignRoleAsync(user, ProjectConstant.UserRoles.FreelancePT);
                await SendAccountInformationEmail(user, request.Password, request.IsTestAccount, ProjectConstant.UserRoles.FreelancePT);
                await InsertUserDetail(user);
                break;
            case ProjectConstant.UserRoles.Admin:
                await _applicationUserService.AssignRoleAsync(user, ProjectConstant.UserRoles.Admin);
                break;
        }

        return new RegisterResponseDto { UserId = user.Id };
    }

    public async Task InsertUserDetail(ApplicationUser user)
    {
        var userDetail = new UserDetail { Id = user.Id };
        _unitOfWork.Repository<UserDetail>().Insert(userDetail);
        await _unitOfWork.CommitAsync();
    }

    public async Task SendAccountInformationEmail(ApplicationUser user, string password, bool isTestAccount, string role)
    {
        if (isTestAccount)
        {
            return;
        }
        await emailService.SendAccountInformationEmailAsync(user, password, role);
    }
}
