using System;
using FitBridge_Application.Dtos.SessionActivities;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities.GetSessionActivityById;

public class GetSessionActivityByIdQuery : IRequest<SessionActivityResponseDto>
{
    public Guid Id { get; set; }
}
