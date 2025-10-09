using System;
using MediatR;
using FitBridge_Application.Dtos.Bookings;

namespace FitBridge_Application.Features.Bookings.RequestEditBooking;

public class RequestEditBookingCommand : IRequest<EditBookingResponseDto>
{
    public Guid TargetBookingId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? BookingName { get; set; }
    public string? Note { get; set; }
}
