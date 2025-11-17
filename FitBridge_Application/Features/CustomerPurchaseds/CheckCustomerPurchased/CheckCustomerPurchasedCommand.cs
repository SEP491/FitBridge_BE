using System;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.CheckCustomerPurchased;

public class CheckCustomerPurchasedCommand : IRequest<Guid>
{
    public Guid? PtId { get; set; }

    public Guid? CustomerId { get; set; }
}