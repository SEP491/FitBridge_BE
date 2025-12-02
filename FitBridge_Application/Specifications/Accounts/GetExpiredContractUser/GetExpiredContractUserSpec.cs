using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Contracts;

namespace FitBridge_Application.Specifications.Accounts.GetExpiredContractUser;

public class GetExpiredContractUserSpec : BaseSpecification<ApplicationUser>
{
    public GetExpiredContractUserSpec(GetExpiredContractUserParams parameters, List<Guid> UserIds) : base(x => (parameters.CustomerId == null || x.Id == parameters.CustomerId)
    && (x.ContractRecords.Count == 0 || !x.ContractRecords.Any(x => x.EndDate > DateOnly.FromDateTime(DateTime.UtcNow)))
    && UserIds.Contains(x.Id))
    {
        AddInclude(x => x.ContractRecords);
        if (parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
