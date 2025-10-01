using System;

namespace FitBridge_Application.Specifications.Bookings.GetCustomerBookings;

public class GetCustomerBookingsParams : BaseParams
{
    public Guid CustomerId { get; set; }
}
