using System;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings;

public class CreateRequestBookingResponseDto
{
    public Guid Id { get; set; }

    public string? BookingName { get; set; }

    public DateOnly BookingDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public Guid CustomerId { get; set; }

    public RequestType RequestType { get; set; }
}