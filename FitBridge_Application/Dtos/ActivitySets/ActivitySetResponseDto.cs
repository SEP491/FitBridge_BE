using System;

namespace FitBridge_Application.Dtos.ActivitySets;

public class ActivitySetResponseDto
{
    public Guid Id { get; set; }
    public int? PlannedPracticeTime { get; set; }
    public double? WeightLifted { get; set; }
    public int? PlannedNumOfReps { get; set; }
    public int? NumOfReps { get; set; }
    public int? PracticeTime { get; set; }
    public bool IsCompleted { get; set; }
}
