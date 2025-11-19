using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Specifications.Orders.GetAllProductOrders;
using MediatR;

namespace FitBridge_Application.Features.Orders.GetAllProductOrder;

public class GetAllProductOrdersQuery : IRequest<PagingResultDto<GetAllProductOrderResponseDto>>
{
    public GetAllProductOrdersParams Params { get; set; } = null!;
}
