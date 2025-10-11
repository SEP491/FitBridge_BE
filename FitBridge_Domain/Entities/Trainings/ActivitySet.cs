using System;

namespace FitBridge_Domain.Entities.Trainings;

public class ActivitySet : BaseEntity
{
    public int? RestTime { get; set; }
    public int? NumOfReps { get; set; }
    public double? WeightLifted { get; set; }
    public int? PracticeTime { get; set; }
    public Guid SessionActivityId { get; set; }
    public SessionActivity SessionActivity { get; set; }
    public bool IsCompleted { get; set; }
}
