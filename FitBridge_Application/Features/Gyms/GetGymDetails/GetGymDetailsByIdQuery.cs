using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Gym;
using MediatR;

namespace FitBridge_Application.Features.Gyms.GetGymDetails
{
    public class GetGymDetailsByIdQuery : IRequest<GetGymDetailsDto>
    {
        public Guid Id { get; set; }
    }
}