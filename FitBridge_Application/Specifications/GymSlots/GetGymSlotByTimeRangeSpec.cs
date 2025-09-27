using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetGymSlotByTimeRangeSpec : BaseSpecification<GymSlot>
{
    public GetGymSlotByTimeRangeSpec(TimeOnly startTime, TimeOnly endTime) : base(x => !(startTime >= x.EndTime || endTime <= x.StartTime)
    && x.IsEnabled)
    {
    }
}
