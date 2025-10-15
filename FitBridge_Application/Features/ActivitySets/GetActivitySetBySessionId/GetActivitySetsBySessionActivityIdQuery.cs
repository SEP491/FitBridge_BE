using System;
using FitBridge_Application.Dtos.ActivitySets;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.GetActivitySetBySessionId;

public class GetActivitySetsBySessionActivityIdQuery : IRequest<List<ActivitySetResponseDto>>
{
    public Guid SessionActivityId { get; set; }
}
