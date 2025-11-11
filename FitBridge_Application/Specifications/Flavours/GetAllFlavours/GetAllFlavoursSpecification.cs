using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Flavours.GetAllFlavours;

public class GetAllFlavoursSpecification : BaseSpecification<Flavour>
{
    public GetAllFlavoursSpecification() : base(x => x.IsEnabled)
    {
        AddOrderBy(x => x.Name);
    }
}
