using System;
using FitBridge_Application.Dtos.ActivitySets;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.CreateActivitySet;

public class CreateActivitySetCommand : IRequest<ActivitySetResponseDto>
{
    public Guid SessionActivityId { get; set; }
    public double? WeightLifted { get; set; }
    public int? NumOfReps { get; set; }
    public double? PlannedPracticeTime { get; set; }
}
