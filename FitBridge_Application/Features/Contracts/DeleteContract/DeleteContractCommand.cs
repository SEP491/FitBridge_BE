using System;
using MediatR;

namespace FitBridge_Application.Features.Contracts.DeleteContract;

public class DeleteContractCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}
