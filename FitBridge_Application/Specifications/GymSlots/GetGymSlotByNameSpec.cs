using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetGymSlotByNameSpec : BaseSpecification<GymSlot>
{
    public GetGymSlotByNameSpec(string name) : base(x => x.Name == name && x.IsEnabled)
    {
    }

}
