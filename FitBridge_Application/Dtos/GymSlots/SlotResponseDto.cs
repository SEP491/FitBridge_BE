using System;

namespace FitBridge_Application.Dtos.GymSlots;

public class SlotResponseDto
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
}
