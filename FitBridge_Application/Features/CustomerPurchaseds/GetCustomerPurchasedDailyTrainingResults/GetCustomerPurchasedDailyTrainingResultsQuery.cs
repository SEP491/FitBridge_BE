using FitBridge_Application.Dtos.CustomerPurchaseds;
using MediatR;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedDailyTrainingResults
{
    public class GetCustomerPurchasedDailyTrainingResultsQuery : IRequest<CustomerPurchasedDailyResultsResponseDto>
    {
        public Guid CustomerPurchasedId { get; set; }
    }
}