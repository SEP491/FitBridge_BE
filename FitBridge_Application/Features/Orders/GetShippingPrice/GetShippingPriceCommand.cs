using System;
using FitBridge_Application.Dtos.Orders;
using MediatR;

namespace FitBridge_Application.Features.Orders.GetShippingPrice;

public class GetShippingPriceCommand : IRequest<ShippingEstimateDto>
{
    public Guid AddressId { get; set; }
}
