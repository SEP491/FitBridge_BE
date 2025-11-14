using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Dtos.Messaging;

public class BookingRequestDto
{
    public Guid BookingRequestId { get; set; }

    public Guid? TargetBookingId { get; set; }

    public Guid CustomerPurchasedId { get; set; }

    public string? Note { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? BookingName { get; set; }

    public string RequestStatus { get; set; }

    public DateOnly BookingDate { get; set; }

    public string RequestType { get; set; }

    public static BookingRequestDto FromEntity(BookingRequest bookingRequest)
    {
        return new BookingRequestDto
        {
            BookingRequestId = bookingRequest.Id,
            RequestStatus = bookingRequest.RequestStatus.ToString(),
            RequestType = bookingRequest.RequestType.ToString(),
            StartTime = bookingRequest.StartTime,
            EndTime = bookingRequest.EndTime,
            BookingDate = bookingRequest.BookingDate,
            TargetBookingId = bookingRequest.TargetBookingId,
            Note = bookingRequest.Note,
            BookingName = bookingRequest.BookingName
        };
    }
}