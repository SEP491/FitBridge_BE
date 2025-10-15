using System;

namespace FitBridge_Application.Dtos.ActivitySets;

public class ActivitySetResponseDto
{
    public Guid Id { get; set; }
    public double? PlannedPracticeTime { get; set; }
    public double? WeightLifted { get; set; }
    public int? PlannedNumOfReps { get; set; }
    public int? NumOfReps { get; set; }
    public double? PracticeTime { get; set; }
    public bool IsCompleted { get; set; }
    public double? RestTime { get; set; }
}
