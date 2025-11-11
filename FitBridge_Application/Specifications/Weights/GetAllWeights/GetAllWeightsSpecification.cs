using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Weights.GetAllWeights;

public class GetAllWeightsSpecification : BaseSpecification<Weight>
{
    public GetAllWeightsSpecification() : base(x => x.IsEnabled)
    {
        AddOrderBy(x => x.Value);
    }
}
