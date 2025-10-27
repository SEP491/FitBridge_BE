using System;
using MediatR;
using FitBridge_Application.Dtos.Gym;

namespace FitBridge_Application.Features.Gyms.GetGymOwnerCustomerById;

public class GetGymOwnerCustomerByIdQuery : IRequest<GetGymOwnerCustomerDetail>
{
    public Guid Id { get; set; }
}
