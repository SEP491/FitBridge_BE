using System;
using MediatR;

namespace FitBridge_Application.Features.Bookings.StartBookingSession;

public class StartBookingSessionCommand : IRequest<DateTime>
{
    public Guid BookingId { get; set; }
}
