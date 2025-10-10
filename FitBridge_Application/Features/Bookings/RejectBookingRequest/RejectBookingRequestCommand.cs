using System;
using MediatR;

namespace FitBridge_Application.Features.Bookings.RejectBookingRequest;

public class RejectBookingRequestCommand : IRequest<bool>
{
    public Guid BookingRequestId { get; set; }
}
