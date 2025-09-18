using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Gym.GetGymPtsByCourse;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetGymPtsByCourse
{
    internal class GetGymPtsQueryByCourseQueryHandler(
        IApplicationUserService applicationUserService,
        IMapper mapper) : IRequestHandler<GetGymPtsByCourseQuery, PagingResultDto<GetGymPtsDto>>
    {
        public async Task<PagingResultDto<GetGymPtsDto>> Handle(GetGymPtsByCourseQuery request, CancellationToken cancellationToken)
        {
            var spec = new GetGymPtsByGymCourseSpecification(request.GymId, request.GetGymPtsByCourseParams);
            var results = await applicationUserService.GetAllUserWithSpecProjectedAsync<GetGymPtsDto>(spec, mapper.ConfigurationProvider);
            var totalItems = await applicationUserService.CountAsync(spec);

            return new PagingResultDto<GetGymPtsDto>(totalItems, results);
        }
    }
}