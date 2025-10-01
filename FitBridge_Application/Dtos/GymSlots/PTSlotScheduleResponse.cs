using System;

namespace FitBridge_Application.Dtos.GymSlots;

public class PTSlotScheduleResponse
{
    public Guid PtGymSlotId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerAvatar { get; set; }
}
