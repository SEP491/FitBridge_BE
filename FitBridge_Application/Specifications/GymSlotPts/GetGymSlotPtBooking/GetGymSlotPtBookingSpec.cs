using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtBooking;

public class GetGymSlotPtBookingSpec : BaseSpecification<PTGymSlot>
{
    public GetGymSlotPtBookingSpec(GetGymSlotPtBookingParams parameters) : base(x => x.PTId == parameters.GymPtId
    && (parameters.FromDate == null || x.RegisterDate >= parameters.FromDate)
    && (parameters.ToDate == null || x.RegisterDate <= parameters.ToDate)
    && x.Booking != null
    && (parameters.SessionStatus == null || x.Booking.SessionStatus == parameters.SessionStatus)
    && (parameters.CustomerId == null || x.Booking.CustomerId == parameters.CustomerId))
    {
        AddOrderByDesc(x => x.RegisterDate);
        AddInclude(x => x.Booking);
        AddInclude(x => x.Booking.Customer);
        AddInclude(x => x.GymSlot);
        AddInclude(x => x.PT);
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
