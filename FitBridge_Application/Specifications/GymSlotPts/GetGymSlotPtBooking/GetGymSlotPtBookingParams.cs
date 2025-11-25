using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtBooking;

public class GetGymSlotPtBookingParams : BaseParams
{
    public Guid? GymPtId { get; set; }
    public Guid? CustomerId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
    public SessionStatus? SessionStatus { get; set; }
}
