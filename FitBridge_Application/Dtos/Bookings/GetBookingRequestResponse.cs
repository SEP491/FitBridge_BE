using System;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings;

public class GetBookingRequestResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid PtId { get; set; }
    public Guid? TargetBookingId { get; set; }
    public Guid CustomerPurchasedId { get; set; }
    public string? Note { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? BookingName { get; set; }
    public RequestType RequestType { get; set; }
    public BookingRequestStatus RequestStatus { get; set; }
    public BookingResponseDto? OriginalBooking { get; set; }
}
