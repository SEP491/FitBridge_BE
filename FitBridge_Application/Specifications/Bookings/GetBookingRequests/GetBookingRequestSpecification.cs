using System;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Application.Specifications;

namespace FitBridge_Application.Specifications.Bookings.GetBookingRequests;

public class GetBookingRequestPtSpecification : BaseSpecification<BookingRequest>
{
    public GetBookingRequestPtSpecification(GetBookingRequestParams parameters, Guid? customerId = null, Guid? ptId = null)
        : base(x => ((x.PtId != null && x.PtId == ptId) || (x.CustomerId != null && x.CustomerId == customerId))
        && parameters.CustomerPurchasedId == x.CustomerPurchasedId)
    {
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
        AddInclude(x => x.TargetBooking);
    }
}
