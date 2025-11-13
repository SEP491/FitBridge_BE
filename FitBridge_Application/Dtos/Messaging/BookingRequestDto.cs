namespace FitBridge_Application.Dtos.Messaging;

public class BookingRequestDto
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

    public string RequestStatus { get; set; }

    public DateOnly BookingDate { get; set; }

    public string RequestType { get; set; }
}
