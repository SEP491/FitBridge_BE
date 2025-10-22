namespace FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;

public class MuscleGroupInsightDto
{
    public string MuscleGroup { get; set; }

    public double TotalTime { get; set; }

    public int SetsCompleted { get; set; }

    public double TotalWeight { get; set; }

    public int SetsCount { get; set; }
}