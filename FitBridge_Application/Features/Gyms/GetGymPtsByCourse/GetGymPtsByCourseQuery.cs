using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym.GetGymPtsByCourse;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetGymPtsByCourse
{
    public class GetGymPtsByCourseQuery(GetGymPtsByGymCourseParams getGymPtsByGymCourseParams, Guid gymId) : IRequest<PagingResultDto<GetGymPtsDto>>
    {
        public Guid GymId { get; set; } = gymId;

        public GetGymPtsByGymCourseParams GetGymPtsByCourseParams { get; set; } = getGymPtsByGymCourseParams;
    }
}