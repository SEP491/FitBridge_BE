using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.Trainings;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities.UpdateSessionActivity;

public class UpdateSessionActivityCommand : IRequest<SessionActivitiyResponseDto>
{
    public Guid SessionActivityId { get; set; }
    public Guid BookingId { get; set; }
    public ActivityType ActivityType { get; set; }
    public string ActivityName { get; set; }
    public MuscleGroupEnum[] MuscleGroups { get; set; } = Array.Empty<MuscleGroupEnum>();
}
