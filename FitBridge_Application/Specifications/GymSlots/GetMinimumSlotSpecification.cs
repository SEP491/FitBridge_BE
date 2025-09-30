using System;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Enums.Gyms;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetMinimumSlotSpecification : BaseSpecification<PTGymSlot>
{
    public GetMinimumSlotSpecification(DateOnly startWeek, DateOnly endWeek, Guid gymPtId) : base(x => startWeek <= x.RegisterDate && endWeek >= x.RegisterDate && x.PTId == gymPtId)
    {
    }
}
