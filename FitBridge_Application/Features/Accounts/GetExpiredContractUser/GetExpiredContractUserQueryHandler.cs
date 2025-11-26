using System;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Contracts;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Accounts.GetExpiredContractUser;
using FitBridge_Domain.Entities.Identity;
using AutoMapper;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetExpiredContractUser;

public class GetExpiredContractUserQueryHandler(IUnitOfWork _unitOfWork, IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetExpiredContractUserQuery, PagingResultDto<NonContractUserDto>>
{
    public async Task<PagingResultDto<NonContractUserDto>> Handle(GetExpiredContractUserQuery request, CancellationToken cancellationToken)
    {
        var customerIds = new List<Guid>();
        var gymOwners = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.GymOwner);
        foreach (var gymOwner in gymOwners)
        {
            customerIds.Add(gymOwner.Id);
        }
        var freelancePts = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.FreelancePT);
        foreach (var freelancePt in freelancePts)
        {
            customerIds.Add(freelancePt.Id);
        }
        var spec = new GetExpiredContractUserSpec(request.Params, customerIds);
        var users = await _applicationUserService.GetAllUsersWithSpecAsync(spec);
        var result = new List<NonContractUserDto>();
        foreach (var user in users)
        {
            var nonContractUserDto = _mapper.Map<NonContractUserDto>(user);
            nonContractUserDto.Role = await _applicationUserService.GetUserRoleAsync(user);
            result.Add(nonContractUserDto);
        }
        
        var count = await _applicationUserService.CountAsync(spec);
        return new PagingResultDto<NonContractUserDto>(count, result);
    }

}
