using System;

namespace FitBridge_Application.Dtos.GymSlots;

public class GetPTSlotResponse
{
    public Guid PtGymSlotId { get; set; }
    public bool? IsBooking { get; set; }
    public string? CustomerName { get; set; }
}
