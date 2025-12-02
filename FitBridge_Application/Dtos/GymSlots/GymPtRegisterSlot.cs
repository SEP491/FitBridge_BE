using System;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Dtos.GymSlots;

public class GymPtRegisterSlot
{
    public Guid PtGymSlotId { get; set; }
    public Guid GymSlotId { get; set; }
    public DateOnly RegisterDate { get; set; }
    public PTGymSlotStatus Status { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string SlotName { get; set; }
}
