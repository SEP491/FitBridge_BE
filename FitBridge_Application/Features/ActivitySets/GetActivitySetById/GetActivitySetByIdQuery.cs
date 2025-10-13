using System;
using FitBridge_Application.Dtos.ActivitySets;
using MediatR;

namespace FitBridge_Application.Features.ActivitySets.GetActivitySetById;

public class GetActivitySetByIdQuery : IRequest<ActivitySetResponseDto>
{
    public Guid Id { get; set; }
}
