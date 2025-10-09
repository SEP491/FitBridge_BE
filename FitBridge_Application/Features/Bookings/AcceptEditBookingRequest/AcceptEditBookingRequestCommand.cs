using System;
using FitBridge_Application.Dtos.Bookings;
using MediatR;

namespace FitBridge_Application.Features.Bookings.AcceptEditBookingRequest;

public class AcceptEditBookingRequestCommand : IRequest<UpdateBookingResponseDto>
{
    public Guid BookingRequestId { get; set; }
}
