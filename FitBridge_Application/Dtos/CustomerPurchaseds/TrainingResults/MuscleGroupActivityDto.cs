namespace FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;

public class MuscleGroupActivityDto
{
    public string MuscleGroup { get; set; }

    public int SetsCount { get; set; }

    public int SetsCompleted { get; set; }

    public double TotalWeight { get; set; }

    public int TotalReps { get; set; }

    public double AverageSessionTimeSeconds { get; set; }

    public double AverageWeightLifted { get; set; }

    public double AverageSetsPerSession { get; set; }

    public List<DailyTrainingResultsDto> DailyResults { get; set; } = new();
}