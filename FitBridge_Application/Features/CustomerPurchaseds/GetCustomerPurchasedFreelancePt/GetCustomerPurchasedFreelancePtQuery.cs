using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedFreelancePt;

public class GetCustomerPurchasedFreelancePtQuery : IRequest<PagingResultDto<CustomerPurchasedFreelancePtResponseDto>>
{
    public GetCustomerPurchasedParams Params { get; set; }
}
