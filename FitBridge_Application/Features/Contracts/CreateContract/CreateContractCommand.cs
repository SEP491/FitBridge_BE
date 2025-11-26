using System;
using FitBridge_Domain.Enums.Contracts;
using MediatR;

namespace FitBridge_Application.Features.Contracts.CreateContract;

public class CreateContractCommand : IRequest<Guid>
{
    public Guid CustomerId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
