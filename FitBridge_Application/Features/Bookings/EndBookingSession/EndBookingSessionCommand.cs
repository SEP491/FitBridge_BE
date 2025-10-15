using System;
using MediatR;

namespace FitBridge_Application.Features.Bookings.EndBookingSession;

public class EndBookingSessionCommand : IRequest<DateTime>
{
    public Guid BookingId { get; set; }
}
