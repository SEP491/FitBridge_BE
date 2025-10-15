using System;
using FitBridge_Application.Dtos.SessionActivities;
using MediatR;

namespace FitBridge_Application.Features.SessionActivities.SessionPracticeContent;

public class SessionPracticeContentCommand : IRequest<SessionPracticeContentDto>
{
    public Guid BookingId { get; set; }
}
