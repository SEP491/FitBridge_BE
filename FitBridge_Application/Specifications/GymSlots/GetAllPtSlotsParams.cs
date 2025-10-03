using System;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetAllPtSlotsParams : BaseParams
{
    public Guid PtId { get; set; }
    public DateOnly RegisterDate { get; set; }
}
