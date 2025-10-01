using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlots;

public class GetGymSlotForPtRegisterSpec : BaseSpecification<GymSlot>
    {
    public GetGymSlotForPtRegisterSpec(Guid gymOwnerId, GetAllPtSlotsParams parameters) : base(x => x.GymOwnerId == gymOwnerId && x.IsEnabled)
    {
        AddInclude(x => x.PTGymSlots);
        AddInclude("PTGymSlots.Booking");
        AddInclude("PTGymSlots.Booking.Customer");
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
