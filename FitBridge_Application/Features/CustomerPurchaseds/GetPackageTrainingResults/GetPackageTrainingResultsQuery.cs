using MediatR;
using FitBridge_Application.Dtos.TrainingResults;

namespace FitBridge_Application.Features.CustomerPurchaseds.GetPackageTrainingResults;

public class GetPackageTrainingResultsQuery : IRequest<CustomerPurchasedAnalyticsDto>
{
    public Guid CustomerPurchasedId { get; set; }
}