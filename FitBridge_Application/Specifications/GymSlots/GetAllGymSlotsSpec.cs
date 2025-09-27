using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetAllGymSlotsSpec : BaseSpecification<GymSlot>
{
    public GetAllGymSlotsSpec(GetGymSlotParams parameters, Guid gymId) : base(x => x.IsEnabled && x.GymOwnerId == gymId)
    {
        AddOrderBy(x => x.StartTime);
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }

}
