using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetAllCustomersForAdmin;

public class GetAllCustomersForAdminSpec : BaseSpecification<ApplicationUser>
{
    public GetAllCustomersForAdminSpec(GetAllCustomersForAdminParams parameters) : base(x => x.IsActive)
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
