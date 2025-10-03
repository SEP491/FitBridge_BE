using System;

namespace FitBridge_Application.Dtos.GymSlots;

public class GetPtGymSlotForBookingResponse
{
    public Guid PtGymSlotId { get; set; }
    public string? SlotName { get; set; }   
    public Guid SlotId { get; set; }
    public Guid PTId { get; set; }
    public string? PtName { get; set; }
    public string? AvatarUrl { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public DateOnly? RegisterDate { get; set; }
}
