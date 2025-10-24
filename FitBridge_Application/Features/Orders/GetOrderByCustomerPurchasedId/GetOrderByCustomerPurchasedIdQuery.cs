using System;
using FitBridge_Application.Dtos.Orders;
using MediatR;

namespace FitBridge_Application.Features.Orders.GetOrderByCustomerPurchasedId;

public class GetOrderByCustomerPurchasedIdQuery : IRequest<OrderResponseDto>
{
    public Guid CustomerPurchasedId { get; set; }
}
