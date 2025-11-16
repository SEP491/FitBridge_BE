using System;
using MediatR;

namespace FitBridge_Application.Features.Addresses.DeleteAddress;

public class DeleteAddressCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; set; } = id;
}
