using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.SessionActivities.GetSessionActivitiesByBookingId;

public class GetSessionActivitiesByBookingIdSpecification : BaseSpecification<SessionActivity>
{
    public GetSessionActivitiesByBookingIdSpecification(Guid bookingId) : base(x => x.BookingId == bookingId)
    {
        AddInclude(x => x.ActivitySets);
        AddInclude(x => x.Booking);
    }
}
