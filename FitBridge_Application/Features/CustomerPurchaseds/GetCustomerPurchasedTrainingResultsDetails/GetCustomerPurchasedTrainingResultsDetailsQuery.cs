using FitBridge_Application.Dtos.CustomerPurchaseds;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedTrainingResultsDetails
{
    public class GetCustomerPurchasedTrainingResultsDetailsQuery : IRequest<CustomerPurchasedTrainingResultsDetailResponseDto>
    {
        public Guid CustomerPurchasedId { get; set; }
    }
}