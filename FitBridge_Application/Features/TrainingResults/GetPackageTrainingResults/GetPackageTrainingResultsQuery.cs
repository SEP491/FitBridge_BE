using FitBridge_Application.Dtos.TrainingResults;
using MediatR;

namespace FitBridge_Application.Features.TrainingResults.GetPackageTrainingResults
{
    public class GetPackageTrainingResultsQuery : IRequest<CustomerPurchasedAnalyticsDto>
    {
        public Guid CustomerPurchasedId { get; set; }
    }
}