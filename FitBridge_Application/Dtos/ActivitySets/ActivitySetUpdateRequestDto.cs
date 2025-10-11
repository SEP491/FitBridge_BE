using System;

namespace FitBridge_Application.Dtos.ActivitySets;

public class ActivitySetUpdateRequestDto
{
    public Guid ActivitySetId { get; set; }
    public double? WeightLifted { get; set; }
    public int? NumOfReps { get; set; }
    public bool IsCompleted { get; set; }
    public int? PracticeTime { get; set; }
    public int? RestTime { get; set; }
}
