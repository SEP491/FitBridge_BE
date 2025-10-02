using System;
using FitBridge_Application.Specifications;
using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Specifications.Bookings.GetGymSlotForBooking;

public class GetGymSlotForBookingParams : BaseParams
{
    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public Guid PtId { get; set; }
}
