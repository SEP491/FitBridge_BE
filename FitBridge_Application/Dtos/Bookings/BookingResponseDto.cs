using System;

namespace FitBridge_Application.Dtos.Bookings;

public class BookingResponseDto
{
    public DateOnly BookingDate { get; set; }

    public TimeOnly? PtFreelanceStartTime { get; set; }

    public TimeOnly? PtFreelanceEndTime { get; set; }
    
    public string? BookingName { get; set; }

    public string? Note { get; set; }
}
