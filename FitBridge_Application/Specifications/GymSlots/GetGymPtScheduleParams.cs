using System;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetGymPtScheduleParams : BaseParams
{
    public Guid PtId { get; set; }
    public DateOnly Date { get; set; }
}
