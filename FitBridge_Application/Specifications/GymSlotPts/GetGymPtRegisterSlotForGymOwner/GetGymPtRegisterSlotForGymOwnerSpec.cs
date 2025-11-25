using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.GymSlotPts.GetGymPtRegisterSlotForGymOwner;

public class GetGymPtRegisterSlotForGymOwnerSpec : BaseSpecification<PTGymSlot>
{
    public GetGymPtRegisterSlotForGymOwnerSpec(GetGymPtRegisterSlotForGymOwnerParams parameters) : base(x => x.PTId == parameters.GymPtId &&
     (parameters.FromDate == null || x.RegisterDate >= parameters.FromDate)
     && (parameters.ToDate == null || x.RegisterDate <= parameters.ToDate)
     && x.IsEnabled)
    {
        AddOrderByDesc(x => x.RegisterDate);
        AddInclude(x => x.GymSlot);
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
