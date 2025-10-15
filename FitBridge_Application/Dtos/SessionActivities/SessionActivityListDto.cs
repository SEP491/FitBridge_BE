using System;
using FitBridge_Domain.Enums.ActivitySets;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.SessionActivities;

public class SessionActivityListDto
{
    public Guid Id { get; set; }
    public ActivityType ActivityType { get; set; }
    public string ActivityName { get; set; }
    public List<string> MuscleGroups { get; set; } = new List<string>();
    public int NumOfReps { get; set; }
    public double? WeightLifted { get; set; }
    public int TotalSets { get; set; }
    public ActivitySetType ActivitySetType { get; set; }
    public int TotalPlannedNumOfReps { get; set; }
    public double TotalPlannedPracticeTime { get; set; }
}
