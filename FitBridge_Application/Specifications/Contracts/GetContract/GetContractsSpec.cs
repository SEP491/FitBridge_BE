using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Contracts;

namespace FitBridge_Application.Specifications.Contracts.GetContract;

public class GetContractsSpec : BaseSpecification<ContractRecord>
{
    public GetContractsSpec(GetContractsParams parameters) : base(x => x.IsEnabled
    &&
    (parameters.CustomerId == null || x.CustomerId == parameters.CustomerId)
    )
    {
        AddInclude(x => x.Customer);
        AddOrderByDesc(x => x.CreatedAt);
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        } else {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
