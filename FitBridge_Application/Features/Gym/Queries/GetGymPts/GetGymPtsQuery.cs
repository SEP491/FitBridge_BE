using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym;
using MediatR;

namespace FitBridge_Application.Features.Gym.Queries.GetGymPts
{
    public class GetGymPtsQuery(GetGymPtsParams getGymPtsParams, Guid gymId) : IRequest<PagingResultDto<GetGymPtsDto>>
    {
        public Guid GymId { get; set; } = gymId;

        public GetGymPtsParams GetGymPtsParams { get; set; } = getGymPtsParams;
    }
}