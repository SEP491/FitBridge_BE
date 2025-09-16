using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym;
using MediatR;

namespace FitBridge_Application.Features.Gym.Queries.GetAllGyms
{
    public class GetAllGymsQuery(GetAllGymsParams getAllGymsParams) : IRequest<PagingResultDto<GetAllGymsDto>>
    {
        public GetAllGymsParams Params { get; set; } = getAllGymsParams;
    }
}