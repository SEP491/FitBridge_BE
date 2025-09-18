using AutoMapper;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Gym.GetGymById;
using MediatR;
using System.Net;

namespace FitBridge_Application.Features.Gyms.GetGymDetails
{
    internal class GetGymDetailsByIdQueryHandler(
        IApplicationUserService applicationUserService,
        IMapper mapper) : IRequestHandler<GetGymDetailsByIdQuery, GetGymDetailsDto>
    {
        public async Task<GetGymDetailsDto> Handle(GetGymDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await applicationUserService.GetUserWithSpecProjectedAsync<GetGymDetailsDto>(
                new GetGymByIdSpecification(
                    request.Id),
                    mapper.ConfigurationProvider);

            return dto is null ? throw new KeyNotFoundException($"Gym with id {request.Id} not found.")
                : dto;
        }
    }
}