using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Gym.GetAllGyms;
using MediatR;

namespace FitBridge_Application.Features.Gym.Queries.GetAllGyms
{
    internal class GetAllGymsQueryHandler(
        IApplicationUserService applicationUserService,
        IMapper mapper) : IRequestHandler<GetAllGymsQuery, PagingResultDto<GetAllGymsDto>>
    {
        public async Task<PagingResultDto<GetAllGymsDto>> Handle(GetAllGymsQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetAllGymsSpecification(request.Params);
            var result = await applicationUserService.GetAllUserWithSpecProjectedAsync<GetAllGymsDto>(
                spec,
                mapper.ConfigurationProvider);
            var totalGymsCount = await applicationUserService.CountAsync(spec);

            return new PagingResultDto<GetAllGymsDto>(totalGymsCount, result);
        }
    }
}