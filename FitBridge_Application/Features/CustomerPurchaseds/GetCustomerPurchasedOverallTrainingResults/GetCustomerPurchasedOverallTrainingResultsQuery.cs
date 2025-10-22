using MediatR;
using FitBridge_Application.Dtos.CustomerPurchaseds;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetCustomerPurchasedOverallTrainingResults;

public class GetCustomerPurchasedOverallTrainingResultsQuery : IRequest<CustomerPurchasedOverallResultResponseDto>
{
    public Guid CustomerPurchasedId { get; set; }
}