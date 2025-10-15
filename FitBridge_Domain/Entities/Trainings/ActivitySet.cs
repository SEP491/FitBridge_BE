using System;
using FitBridge_Domain.Enums.ActivitySets;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Domain.Entities.Trainings;

public class ActivitySet : BaseEntity
{
    public double? RestTime { get; set; }
    public int? NumOfReps { get; set; }
    public int? PlannedNumOfReps { get; set; }
    public double? PlannedPracticeTime { get; set; }
    public double? WeightLifted { get; set; }
    public double? PracticeTime { get; set; }
    public Guid SessionActivityId { get; set; }
    public SessionActivity SessionActivity { get; set; }
    public bool IsCompleted { get; set; }
}
