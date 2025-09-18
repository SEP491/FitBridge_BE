using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Specifications.Gym.GetAllGyms;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetAllGyms
{
    public class GetAllGymsQuery(GetAllGymsParams getAllGymsParams) : IRequest<PagingResultDto<GetAllGymsDto>>
    {
        public GetAllGymsParams Params { get; set; } = getAllGymsParams;
    }
}