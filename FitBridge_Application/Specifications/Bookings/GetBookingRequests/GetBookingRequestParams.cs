using System;
using FitBridge_Domain.Enums.Trainings;

namespace FitBridge_Application.Specifications.Bookings.GetBookingRequests;

public class GetBookingRequestParams : BaseParams
{
    public Guid CustomerPurchasedId { get; set; }
}
