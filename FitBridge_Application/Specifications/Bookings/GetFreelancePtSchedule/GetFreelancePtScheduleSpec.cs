using System;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetFreelancePtSchedule;

public class GetFreelancePtScheduleSpec : BaseSpecification<Booking>
{
    public GetFreelancePtScheduleSpec(GetFreelancePtScheduleParams parameters, Guid PtId) : base(x => x.PtId == PtId && x.BookingDate == parameters.Date)
    {
        AddInclude(x => x.Customer);
        AddInclude(x => x.CustomerPurchased);
        AddInclude(x => x.CustomerPurchased.OrderItems);
        AddInclude("CustomerPurchased.OrderItems.FreelancePTPackage");

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
