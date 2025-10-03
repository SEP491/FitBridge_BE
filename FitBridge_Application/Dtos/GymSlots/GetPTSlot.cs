using System;

namespace FitBridge_Application.Dtos.GymSlots;

public class GetPTSlot
{
    public Guid SlotId { get; set; }
    public string? FullName { get; set; }
    public string? Name { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public bool? IsActivated { get; set; }
    public GetPTSlotResponse PTSlots { get; set; }
}
