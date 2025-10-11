using System;
using FitBridge_Application.Dtos.ActivitySets;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.UpdateActivityProgress;

public class UpdateActivityProgressCommand : IRequest<List<ActivitySetResponseDto>>
{
    public List<ActivitySetUpdateRequestDto> ActivitySets { get; set; }
}
