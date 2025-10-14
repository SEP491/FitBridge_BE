using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtCustomers
{
    public class GetFreelancePtCustomerQuery(GetFreelancePtCustomerPurchasedParams parameters) : IRequest<PagingResultDto<GetCustomersDto>>
    {
        public GetFreelancePtCustomerPurchasedParams Params { get; set; } = parameters;
    }
}