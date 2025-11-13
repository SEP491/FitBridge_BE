using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Products.GetProductDetailForSale;

public class GetProductDetailForSaleSpec : BaseSpecification<Product>
{
    public GetProductDetailForSaleSpec(Guid productId) : base(x => x.IsEnabled && x.Id == productId)
    {
        AddInclude(x => x.ProductDetails);
        AddInclude(x => x.Brand);
        AddInclude(x => x.SubCategory);
        AddInclude("ProductDetails.Reviews");
        AddInclude("ProductDetails.Reviews.User");
        AddInclude("ProductDetails.Weight");
        AddInclude("ProductDetails.Flavour");
    }
}
