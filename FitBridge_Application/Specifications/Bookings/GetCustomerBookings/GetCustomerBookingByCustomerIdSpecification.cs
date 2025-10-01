using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetCustomerBookings;

public class GetCustomerBookingByCustomerIdSpecification : BaseSpecification<Booking>
{
    public GetCustomerBookingByCustomerIdSpecification(GetCustomerBookingsParams parameters) : base(x => x.CustomerId == parameters.CustomerId
    && x.IsEnabled
    && x.SessionStatus != SessionStatus.Cancelled)
    {
        AddInclude(x => x.PTGymSlot);
        AddInclude(x => x.PTGymSlot.GymSlot);
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
