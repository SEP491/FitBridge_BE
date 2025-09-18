using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Gym;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetGymPtsByGymId
{
    internal class GetGymPtsByGymIdQueryHandler(
        IApplicationUserService applicationUserService,
        IMapper mapper) : IRequestHandler<GetGymPtsByGymIdQuery, PagingResultDto<GetGymPtsDto>>
    {
        public async Task<PagingResultDto<GetGymPtsDto>> Handle(GetGymPtsByGymIdQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetGymPtsByGymIdSpecification(request.GymId, request.GetGymPtsByCourseParams);
            var results = await applicationUserService.GetAllUserWithSpecProjectedAsync<GetGymPtsDto>(spec, mapper.ConfigurationProvider);
            var totalItems = await applicationUserService.CountAsync(spec);

            return new PagingResultDto<GetGymPtsDto>(totalItems, results);
        }
    }
}