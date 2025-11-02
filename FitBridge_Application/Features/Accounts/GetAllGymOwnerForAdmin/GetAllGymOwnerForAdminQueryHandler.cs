using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Accounts.GetAllGymOwnerForAdmin;
using MediatR;
using FitBridge_Application.Interfaces.Services;
using AutoMapper;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Application.Features.Accounts.GetAllGymOwnerForAdmin;

public class GetAllGymOwnerForAdminQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetAllGymOwnerForAdminQuery, PagingResultDto<GetAllGymOwnerForAdminDto>>
{
    public async Task<PagingResultDto<GetAllGymOwnerForAdminDto>> Handle(GetAllGymOwnerForAdminQuery request, CancellationToken cancellationToken)
    {
        var userEntity = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.GymOwner);
        var userIds = userEntity.Select(x => x.Id).ToList();
        var spec = new GetAllGymOwnerForAdminSpec(request.Params, userIds);
        var gymOwners = await _applicationUserService.GetAllUserWithSpecProjectedAsync<GetAllGymOwnerForAdminDto>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _applicationUserService.CountAsync(spec);
        return new PagingResultDto<GetAllGymOwnerForAdminDto>(totalItems, gymOwners);
    }
}
