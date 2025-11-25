using System;
using FitBridge_Domain.Enums.Contracts;
using MediatR;

namespace FitBridge_Application.Features.Contracts.CreateContract;

public class CreateContractCommand : IRequest<Guid>
{
    public Guid CustomerId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string FullName { get; set; }
    public string IdentityCardNumber { get; set; }
    public DateOnly IdentityCardDate { get; set; }
    public string IdentityCardPlace { get; set; }
    public string PermanentAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string TaxCode { get; set; }
}
