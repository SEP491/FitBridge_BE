using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Brands.GetAllBrands;

public class GetAllBrandsSpecification : BaseSpecification<Brand>
{
    public GetAllBrandsSpecification() : base(x => x.IsEnabled)
    {
        AddOrderBy(x => x.Name);
    }
}
