using System;
using FitBridge_Application.Dtos.Identities;
using MediatR;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Repositories;
using Microsoft.Extensions.Configuration;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Commons.Constants;
using AutoMapper;

namespace FitBridge_Application.Features.Identities.Registers.RegisterGymPT;

public class RegisterGymPtCommandHandler(IApplicationUserService _applicationUserService, IEmailService _emailService, IConfiguration _configuration, IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<RegisterGymPtCommand, CreateNewPTResponse>
{
    public async Task<CreateNewPTResponse> Handle(RegisterGymPtCommand request, CancellationToken cancellationToken)
    {
        var gymOwner = await _applicationUserService.GetByIdAsync(Guid.Parse(request.GymOwnerId ?? Guid.Empty.ToString()));
        if (gymOwner == null)
        {
            throw new Exception("Gym owner not found");
        }
        
        if (!await _applicationUserService.IsInRoleAsync(gymOwner, ProjectConstant.UserRoles.GymOwner) && !await _applicationUserService.IsInRoleAsync(gymOwner, ProjectConstant.UserRoles.Admin))
        {
            throw new Exception("Acccount do not have Gym Owner or Admin role to create Gym PT");
        }
        var user = _mapper.Map<ApplicationUser>(request);

        await _applicationUserService.InsertUserAsync(user, request.Password);

        var userDetail = new UserDetail {
            Id = user.Id,
            Weight = request.CreateNewPT.Weight,
            Height = request.CreateNewPT.Height,
            Experience = request.CreateNewPT.Experience
        };

        _unitOfWork.Repository<UserDetail>().Insert(userDetail);

        await _applicationUserService.AssignRoleAsync(user, ProjectConstant.UserRoles.GymPT);
        var isTestAccount = request.IsTestAccount ?? false;
        if (isTestAccount != true)
        {
            await _emailService.SendAccountInformationEmailAsync(user, request.Password, ProjectConstant.UserRoles.GymPT);
        }

        await _unitOfWork.CommitAsync();

        return _mapper.Map<CreateNewPTResponse>(request);
    }
}
