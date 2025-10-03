using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.Bookings.GetGymSlotForBooking;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Specifications.GymSlotPts.GetPTGymSlotForBooking;

public class GetPTGymSlotForBookingSpecification : BaseSpecification<PTGymSlot>
{
    public GetPTGymSlotForBookingSpecification(GetGymSlotForBookingParams parameters)
        : base(x => x.PTId == parameters.PtId
        && x.PTId == parameters.PtId
        && x.RegisterDate == parameters.Date
        && x.IsEnabled
        && x.Booking == null)
    {
        AddInclude(x => x.GymSlot);
        AddInclude(x => x.PT);
    }   
}
