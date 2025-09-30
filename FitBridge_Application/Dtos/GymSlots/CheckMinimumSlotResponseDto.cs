using System;

namespace FitBridge_Application.Dtos.GymSlots;

public class CheckMinimumSlotResponseDto
{
    public int MinimumSlot { get; set; }
    public int RegisteredSlot { get; set; }
    public bool IsAccepted { get; set; }
}
