using System;

namespace FitBridge_Application.Dtos.Bookings;

public class EditBookingResponseDto
{
    public Guid BookingRequestId { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly OriginalStartTime { get; set; }
    public TimeOnly OriginalEndTime { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? BookingName { get; set; }
    public string? Note { get; set; }
    public string? RequestType { get; set; }
    public string? RequestStatus { get; set; }
    public Guid TargetBookingId { get; set; }
}
