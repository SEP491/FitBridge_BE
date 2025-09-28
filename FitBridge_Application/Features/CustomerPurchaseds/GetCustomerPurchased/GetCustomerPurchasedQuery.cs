using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchased;

public class GetCustomerPurchasedQuery : IRequest<PagingResultDto<CustomerPurchasedResponseDto>>
{
    public GetCustomerPurchasedParams Params { get; set; }
}
