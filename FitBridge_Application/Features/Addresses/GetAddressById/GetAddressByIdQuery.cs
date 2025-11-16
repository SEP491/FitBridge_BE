using System;
using FitBridge_Application.Dtos.Addresses;
using MediatR;

namespace FitBridge_Application.Features.Addresses.GetAddressById;

public class GetAddressByIdQuery(Guid id) : IRequest<AddressResponseDto>
{
    public Guid Id { get; set; } = id;
}
