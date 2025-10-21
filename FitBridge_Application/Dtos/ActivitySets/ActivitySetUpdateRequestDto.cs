using System;

namespace FitBridge_Application.Dtos.ActivitySets;

public class ActivitySetUpdateRequestDto
{
    public Guid ActivitySetId { get; set; }
    public double? WeightLifted { get; set; }
    public int? NumOfReps { get; set; }
    public bool IsCompleted { get; set; }
    public double? PracticeTime { get; set; }
    public int? ActualDistance { get; set; }
    public double? RestTime { get; set; }
}
