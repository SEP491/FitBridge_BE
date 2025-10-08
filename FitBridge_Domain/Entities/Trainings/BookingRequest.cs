using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Domain.Entities.Trainings;

public class BookingRequest : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid PtId { get; set; }
    public Guid? TargetBookingId { get; set; }
    public Guid CustomerPurchasedId { get; set; }
    public string? Note { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? BookingName { get; set; }
    public BookingRequestStatus RequestStatus { get; set; }
    public DateOnly BookingDate { get; set; }
    public RequestType RequestType { get; set; }
    public ApplicationUser Customer { get; set; }
    public ApplicationUser Pt { get; set; }
    public Booking? TargetBooking { get; set; }
    public CustomerPurchased CustomerPurchased { get; set; }
    
}
