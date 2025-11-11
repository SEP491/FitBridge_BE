using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Products.GetProductForSales;

public class GetProductForSaleSpec : BaseSpecification<Product>
{
    public GetProductForSaleSpec(GetProductForSaleParams parameters) : base(x =>
    x.IsDisplayed
    && x.ProductDetails.Any()
    && ((parameters.FromPrice == null) || parameters.FromPrice <= 0 || x.ProductDetails.Min(pd => pd.SalePrice) >= parameters.FromPrice.Value)
    && ((parameters.ToPrice == null) || parameters.ToPrice <= 0 || x.ProductDetails.Min(pd => pd.SalePrice) <= parameters.ToPrice.Value)
    && ((parameters.Rating == null) || parameters.Rating <= 0 || x.ProductDetails.SelectMany(x => x.Reviews).Any() || x.ProductDetails.SelectMany(pd => pd.Reviews).Average(r => r.Rating) >= parameters.Rating.Value)
    && ((parameters.BrandId == null) || x.BrandId == parameters.BrandId.Value)
    && ((parameters.SubCategoryId == null) || x.SubCategoryId == parameters.SubCategoryId.Value)
    && (string.IsNullOrEmpty(parameters.SearchTerm) || x.Name.ToLower().Contains(parameters.SearchTerm.ToLower()))
    && (parameters.CategoryId == null || x.SubCategory.CategoryId == parameters.CategoryId.Value))
    {
        AddInclude(x => x.ProductDetails);
        if (parameters.SortOrder.ToLower() == "asc")
        {
            AddOrderBy(x => x.ProductDetails.Min(pd => pd.SalePrice));
        }
        else
        {
            AddOrderByDesc(x => x.ProductDetails.Min(pd => pd.SalePrice));
        }
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
    }
}
