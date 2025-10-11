using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.SessionActivities;

public class SessionActivitiyResponseDto
{
    public Guid Id { get; set; }
    public ActivityType ActivityType { get; set; }

    public string ActivityName { get; set; }

    public List<string> MuscleGroups { get; set; } = new List<string>();

    public Guid BookingId { get; set; }

    public ICollection<ActivitySetResponseDto> ActivitySets { get; set; } = new List<ActivitySetResponseDto>();
}
