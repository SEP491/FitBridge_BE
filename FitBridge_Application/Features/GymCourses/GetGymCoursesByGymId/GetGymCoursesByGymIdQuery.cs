using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym.GetGymCoursesByGymId;
using MediatR;

namespace FitBridge_Application.Features.GymCourses.GetGymCoursesByGymId
{
    public class GetGymCoursesByGymIdQuery(Guid gymId, GetGymCourseByGymIdParams getGymCoursesByGymIdParams) : IRequest<PagingResultDto<GetGymCourseDto>>
    {
        public Guid GymId { get; set; } = gymId;

        public GetGymCourseByGymIdParams GetGymCoursesByGymIdParams { get; set; } = getGymCoursesByGymIdParams;
    }
}