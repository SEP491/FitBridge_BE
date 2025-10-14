using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Specifications.Accounts.GetFreelancePtCustomers;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetFreelancePtCustomerPurchased;
using MediatR;

namespace FitBridge_Application.Features.Accounts.GetFreelancePtCustomers
{
    public class GetFreelancePtCustomerQuery(GetFreelancePtCustomerParams parameters) : IRequest<PagingResultDto<GetCustomersDto>>
    {
        public GetFreelancePtCustomerParams Params { get; set; } = parameters;
    }
}