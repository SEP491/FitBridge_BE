using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Domain.Enums.ActivitySets;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.SessionActivities;

public class SessionActivityResponseDto
{
    public Guid Id { get; set; }
    public ActivityType ActivityType { get; set; }
    public ActivitySetType ActivitySetType { get; set; }

    public string ActivityName { get; set; }
    public MuscleGroupEnum MuscleGroup { get; set; }

    public Guid BookingId { get; set; }
    public ICollection<ActivitySetResponseDto> ActivitySets { get; set; } = new List<ActivitySetResponseDto>();
}
