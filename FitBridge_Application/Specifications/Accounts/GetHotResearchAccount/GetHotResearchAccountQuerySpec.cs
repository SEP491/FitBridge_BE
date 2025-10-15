using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetHotResearchAccount;

public class GetHotResearchAccountQuerySpec : BaseSpecification<ApplicationUser>
{
    public GetHotResearchAccountQuerySpec(GetHotResearchAccountParams parameters) : base(x => x.hotResearch)
    {
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        } else {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
