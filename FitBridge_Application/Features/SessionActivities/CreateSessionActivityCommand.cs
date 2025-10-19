using System;
using System.ComponentModel.DataAnnotations;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Domain.Enums.ActivitySets;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.Trainings;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities;

public class CreateSessionActivityCommand : IRequest<SessionActivityResponseDto>
{
    public Guid BookingId { get; set; }
    public ActivityType ActivityType { get; set; }
    public ActivitySetType ActivitySetType { get; set; }
    public string ActivityName { get; set; }
    public MuscleGroupEnum MuscleGroup { get; set; }
    public List<ActivitySetRequestDto> ActivitySets { get; set; } = new List<ActivitySetRequestDto>();
}
