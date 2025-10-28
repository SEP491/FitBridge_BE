using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.GymPTs;
using FitBridge_Application.Specifications.Accounts.GetAllGymPts;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetAllGymPts
{
    public class GetAllGymPtsQuery : IRequest<PagingResultDto<GetAllGymPtsResponseDto>>
    {
        public GetAllGymPtsParams Params { get; set; }
    }
}