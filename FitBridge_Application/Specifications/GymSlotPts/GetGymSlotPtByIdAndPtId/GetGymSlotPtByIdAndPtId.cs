using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlotPts.GetGymSlotPtByIdAndPtId;

public class GetGymSlotPtByIdAndPtId : BaseSpecification<PTGymSlot>
{
    public GetGymSlotPtByIdAndPtId(Guid gymSlotId, Guid ptId) : base(x => x.GymSlotId == gymSlotId && x.PTId == ptId)
    {
        AddInclude(x => x.Booking);
    }
}
