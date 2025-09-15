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

namespace FitBridge_Application.Features.Identities.Registers.RegisterCustomer;

public class RegisterCommandHandler(IApplicationUserService _applicationUserService, IEmailService emailService, IConfiguration _configuration, IUnitOfWork _unitOfWork) : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            Dob = request.Dob,
            IsMale = request.IsMale,
            Password = request.Password
        };
        await _applicationUserService.InsertUserAsync(user, request.Password);
        await _applicationUserService.AssignRoleAsync(user, ProjectConstant.UserRoles.Customer);
        
        var userDetail = new UserDetail { Id = user.Id };
        _unitOfWork.Repository<UserDetail>().Insert(userDetail);
        await _unitOfWork.CommitAsync();

        var token = await _applicationUserService.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{_configuration["FrontendUrl"]}/confirm-email?token={token}&email={user.Email}";
        await emailService.SendRegistrationConfirmationEmailAsync(user.Email, confirmationLink, user.FullName);
        return new RegisterResponseDto { Status = "200", Message = "User created successfully", UserId = user.Id };
    }
}
