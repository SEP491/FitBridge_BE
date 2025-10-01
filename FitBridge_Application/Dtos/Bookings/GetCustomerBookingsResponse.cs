using System;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Dtos.Bookings;

public class GetCustomerBookingsResponse
{
    public Guid BookingId { get; set; }
    public DateOnly BookingDate { get; set; }

    public TimeOnly? PtFreelanceStartTime { get; set; }

    public TimeOnly? PtFreelanceEndTime { get; set; }
    public TimeOnly? GymSlotStartTime { get; set; }
    public TimeOnly? GymSlotEndTime { get; set; }
    public Guid? PTGymSlotId { get; set; }

    public Guid CustomerId { get; set; }

    public Guid CustomerPurchasedId { get; set; }

    public SessionStatus SessionStatus { get; set; }

    public string? Note { get; set; }

    public string? NutritionTip { get; set; }
}
