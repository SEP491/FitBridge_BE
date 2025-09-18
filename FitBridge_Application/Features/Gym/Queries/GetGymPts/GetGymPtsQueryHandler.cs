using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Gym;
using MediatR;

namespace FitBridge_Application.Features.Gym.Queries.GetGymPts
{
    internal class GetGymPtsQueryHandler(
        IApplicationUserService applicationUserService,
        IMapper mapper) : IRequestHandler<GetGymPtsQuery, PagingResultDto<GetGymPtsDto>>
    {
        public async Task<PagingResultDto<GetGymPtsDto>> Handle(GetGymPtsQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetGymPtsSpecification(request.GymId, request.GetGymPtsParams);
            var results = await applicationUserService.GetAllUserWithSpecProjectedAsync<GetGymPtsDto>(spec, mapper.ConfigurationProvider);
            var totalItems = await applicationUserService.CountAsync(spec);

            return new PagingResultDto<GetGymPtsDto>(totalItems, results);
        }
    }
}