namespace FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults
{
    public class DailyTrainingResultsDto
    {
        public DateOnly PracticeDay { get; set; }

        public double TotalWeights { get; set; }

        public int TotalReps { get; set; }

        public double TotalTime { get; set; }
    }
}