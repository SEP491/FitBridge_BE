using System;
using FitBridge_Application.Dtos.ActivitySets;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.UpdateActivitySet;

public class UpdateActivitySetCommand : IRequest<ActivitySetResponseDto>
{
    public Guid ActivitySetId { get; set; }
    public double? WeightLifted { get; set; }
    public int? NumOfReps { get; set; }
}
