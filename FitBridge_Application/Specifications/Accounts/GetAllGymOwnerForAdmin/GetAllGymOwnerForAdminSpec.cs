using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetAllGymOwnerForAdmin;

public class GetAllGymOwnerForAdminSpec : BaseSpecification<ApplicationUser>    
{
    public GetAllGymOwnerForAdminSpec(GetAllGymOwnerForAdminParams parameters, List<Guid> userIds) : base(x =>
    userIds.Contains(x.Id) && x.IsEnabled)
    {
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
