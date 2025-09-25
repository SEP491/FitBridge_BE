using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym.GetGymPtsByGymId;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetGymPtsByGymId
{
    public class GetGymPtsByGymIdQuery(Guid GymId, GetGymPtsByGymIdParams GetGymPtsByCourseParams) : IRequest<PagingResultDto<GetGymPtsDto>>
    {
        public Guid GymId { get; set; } = GymId;

        public GetGymPtsByGymIdParams GetGymPtsByCourseParams { get; set; } = GetGymPtsByCourseParams;
    }
}