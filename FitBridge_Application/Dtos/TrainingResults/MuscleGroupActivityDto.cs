namespace FitBridge_Application.Dtos.TrainingResults;

public class MuscleGroupActivityDto
{
    public string MuscleGroup { get; set; }
    public int ActivityCount { get; set; }
    public int SetsCompleted { get; set; }
    public double TotalWeight { get; set; }
    public int TotalReps { get; set; }
}
