using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedForFreelancePt;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;

public class GetCustomerPurchasedByFreelancePtIdQuery : IRequest<PagingResultDto<GetCustomerPurchasedForFreelancePt>>
{
    public GetCustomerPurchasedForFreelancePtParams Params { get; set; }
}
