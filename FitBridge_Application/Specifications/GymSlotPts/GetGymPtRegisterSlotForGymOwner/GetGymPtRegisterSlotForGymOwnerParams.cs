using System;

namespace FitBridge_Application.Specifications.GymSlotPts.GetGymPtRegisterSlotForGymOwner;

public class GetGymPtRegisterSlotForGymOwnerParams : BaseParams
{
    public Guid GymPtId { get; set; }
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }
}
