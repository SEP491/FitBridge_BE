using System;
using MediatR;

namespace FitBridge_Application.Features.Bookings.CancelGymPtBooking;

public class CancelGymPtBookingCommand : IRequest<bool>
{
    public Guid BookingId { get; set; }
}
