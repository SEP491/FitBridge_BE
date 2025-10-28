using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymPTs;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Accounts.GetAllGymPts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Accounts.GetAllGymPts
{
    internal class GetAllGymPtsQueryHandler(
        IApplicationUserService applicationUserService,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper) : IRequestHandler<GetAllGymPtsQuery, PagingResultDto<GetAllGymPtsResponseDto>>
    {
        public async Task<PagingResultDto<GetAllGymPtsResponseDto>> Handle(GetAllGymPtsQuery request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext!)
                ?? throw new NotFoundException(nameof(ApplicationUser));
            var gymPtsRaw = await applicationUserService.GetUsersByRoleAsync(ProjectConstant.UserRoles.GymPT);
            var spec = new GetAllGymPtsSpec(
                gymPtsRaw.Select(x => x.Id).ToList(),
                accountId,
                queryParams: request.Params);
            var result = await applicationUserService.GetAllUserWithSpecProjectedAsync<GetAllGymPtsResponseDto>(
                spec,
                mapper.ConfigurationProvider);
            var totalGymPtsCount = await applicationUserService.CountAsync(spec);

            return new PagingResultDto<GetAllGymPtsResponseDto>(totalGymPtsCount, result);
        }
    }
}