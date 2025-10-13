using System;
using FitBridge_Application.Dtos.ActivitySets;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.UpdateActivityProgress;

public class UpdateActivityProgressCommand : IRequest<ActivitySetResponseDto>
{
    public ActivitySetUpdateRequestDto ActivitySet { get; set; }
}
