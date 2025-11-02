using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Accounts.GetAllGymPtsForAdmin;
using MediatR;
using FitBridge_Application.Interfaces.Services;
using AutoMapper;
using FitBridge_Application.Commons.Constants;

namespace FitBridge_Application.Features.Accounts.GetAllGymPtsForAdmin;

public class GetAllGymPtsForAdminQueryHandler(IApplicationUserService _applicationUserService, IMapper _mapper) : IRequestHandler<GetAllGymPtsForAdminQuery, PagingResultDto<GetAllGymPtsForAdminResponseDto>>
{
    public async Task<PagingResultDto<GetAllGymPtsForAdminResponseDto>> Handle(GetAllGymPtsForAdminQuery request, CancellationToken cancellationToken)
    {
        var gymPts = await _applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.GymPT);
        var gymPtsIds = gymPts.Select(x => x.Id).ToList();
        var spec = new GetAllGymPtsForAdminSpec(request.Params, gymPtsIds);
        var results = await _applicationUserService.GetAllUserWithSpecProjectedAsync<GetAllGymPtsForAdminResponseDto>(spec, _mapper.ConfigurationProvider);
        var totalItems = await _applicationUserService.CountAsync(spec);
        return new PagingResultDto<GetAllGymPtsForAdminResponseDto>(totalItems, results);
    }
}
