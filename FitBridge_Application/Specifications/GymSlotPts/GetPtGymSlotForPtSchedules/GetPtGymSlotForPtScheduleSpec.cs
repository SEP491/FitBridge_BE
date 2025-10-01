using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymSlots;

namespace FitBridge_Application.Specifications.GymSlotPts.GetPtGymSlotForPtSchedules;

public class GetPtGymSlotForPtScheduleSpec : BaseSpecification<PTGymSlot>
{
    public GetPtGymSlotForPtScheduleSpec(Guid ptId, GetGymPtScheduleParams parameters) : base(x => x.PTId == ptId && x.RegisterDate == parameters.Date && x.IsEnabled && x.Booking != null)
    {
        AddInclude(x => x.GymSlot);
        AddInclude(x => x.Booking);
        AddInclude(x => x.Booking.Customer);
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
