namespace FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;

public class MuscleGroupActivityDto
{
    public string MuscleGroup { get; set; }

    public int SetsCount { get; set; }

    public int SetsCompleted { get; set; }

    public double TotalWeight { get; set; }

    public int TotalReps { get; set; }

    public double TotalTime { get; set; }

    public double AverageSessionTimeSeconds { get; set; }

    public double AverageWeightLiftedPerSession { get; set; }

    public double AverageSetsPerSession { get; set; }

    public double AverageRepsPerSession { get; set; }

    public List<DailyTrainingResultsDto> DailyResults { get; set; } = new();
}