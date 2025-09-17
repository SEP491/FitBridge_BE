using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts;

public class GetAccountByIdSpecificationForUserProfile : BaseSpecification<ApplicationUser>
{
    public GetAccountByIdSpecificationForUserProfile(Guid id) : base(x => x.Id == id)
    {
        AddInclude(x => x.UserDetail);
    }
}
