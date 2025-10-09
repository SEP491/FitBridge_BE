using System;
using MediatR;

namespace FitBridge_Application.Features.Bookings.AcceptBookingRequestCommand;

public class AcceptBookingRequestCommand : IRequest<Guid>
{
    public Guid BookingRequestId { get; set; }
}
