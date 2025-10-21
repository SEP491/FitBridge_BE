using System;

namespace FitBridge_Application.Dtos.ActivitySets;

public class ActivitySetRequestDto
{
    public int? PlannedNumOfReps { get; set; }
    public double? WeightLifted { get; set; }
    public double? PlannedPracticeTime { get; set; }
    public int? PlannedDistance { get; set; }
}
