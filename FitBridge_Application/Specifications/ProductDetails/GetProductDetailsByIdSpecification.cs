using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.ProductDetails;

public class GetProductDetailsByIdSpecification : BaseSpecification<ProductDetail>
{
    public GetProductDetailsByIdSpecification(Guid id) : base(x => x.Id == id && x.IsEnabled)
    {
        AddInclude(x => x.Product);
        AddInclude(x => x.Weight);
        AddInclude(x => x.Flavour);
    }
}
