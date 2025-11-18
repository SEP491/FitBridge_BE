using System;
using System.Text.Json.Serialization;
using MediatR;

namespace FitBridge_Application.Features.Orders.CancelShippingOrder;

public class CancelShippingOrderCommand : IRequest<bool>
{
    [JsonIgnore]
    public Guid OrderId { get; set; }
    public string Comment { get; set; }
}
