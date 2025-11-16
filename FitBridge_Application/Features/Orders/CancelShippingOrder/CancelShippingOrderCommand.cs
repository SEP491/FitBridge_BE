using System;
using MediatR;

namespace FitBridge_Application.Features.Orders.CancelShippingOrder;

public class CancelShippingOrderCommand : IRequest<bool>
{
    public Guid OrderId { get; set; }
    public string Comment { get; set; }
}
