using FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;

namespace FitBridge_Application.Dtos.CustomerPurchaseds
{
    public class CustomerPurchasedDailyResultsResponseDto
    {
        public List<MuscleGroupActivityDto> MuscleGroupActivities { get; set; } = new();
    }
}