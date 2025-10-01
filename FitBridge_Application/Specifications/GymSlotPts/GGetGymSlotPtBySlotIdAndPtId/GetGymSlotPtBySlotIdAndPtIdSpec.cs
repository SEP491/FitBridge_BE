using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlotPts.GGetGymSlotPtBySlotIdAndPtId;

public class GetGymSlotPtBySlotIdAndPtIdSpec : BaseSpecification<PTGymSlot>
{
    public GetGymSlotPtBySlotIdAndPtIdSpec(Guid slotId, Guid ptId, DateOnly registerDate) : base(x => x.GymSlotId == slotId && x.PTId == ptId && x.RegisterDate == registerDate)
    {
    }
}
