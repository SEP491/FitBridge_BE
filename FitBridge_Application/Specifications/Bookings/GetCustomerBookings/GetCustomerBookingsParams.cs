using System;
using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Specifications.Bookings.GetCustomerBookings;

public class GetCustomerBookingsParams : BaseParams
{
    public Guid CustomerId { get; set; }
    [Required]
    public DateOnly Date { get; set; }
}
