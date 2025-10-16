namespace FitBridge_Application.Dtos.TrainingResults;

public class WorkoutStatisticsDto
{
    public double TotalWeightLifted { get; set; }

    public int PlannedNumOfReps { get; set; }

    public double PlannedPracticeTime { get; set; }

    public int TotalRepsCompleted { get; set; }

    public double TotalPracticeTimeSeconds { get; set; }

    public int AverageRestTimeSeconds { get; set; }

    public Dictionary<string, int> ActivityTypeBreakdown { get; set; } = new();
}