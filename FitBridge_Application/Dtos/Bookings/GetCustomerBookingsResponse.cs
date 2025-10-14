using System;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings;

public class GetCustomerBookingsResponse
{
    public Guid BookingId { get; set; }
    public string? BookingName { get; set; }
    public DateOnly BookingDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public Guid? PTGymSlotId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid CustomerPurchasedId { get; set; }

    public SessionStatus SessionStatus { get; set; }
    public string? PtName { get; set; }
    public string? PtAvatarUrl { get; set; }

    public string? Note { get; set; }

    public string? NutritionTip { get; set; }
}
