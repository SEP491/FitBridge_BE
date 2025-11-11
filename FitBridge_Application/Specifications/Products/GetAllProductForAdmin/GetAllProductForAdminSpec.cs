using System;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Products.GetAllProductForAdmin;

public class GetAllProductForAdminSpec : BaseSpecification<Product>
{
    public GetAllProductForAdminSpec(GetAllProductForAdminQueryParams queryParams) : base(x =>
        (queryParams.IsDisplayed == null || x.IsDisplayed == queryParams.IsDisplayed) &&
        (queryParams.ProductId == null || x.Id == queryParams.ProductId) &&
        (queryParams.SubCategoryId == null || x.SubCategoryId == queryParams.SubCategoryId) &&
        (queryParams.BrandId == null || x.BrandId == queryParams.BrandId) &&
        (string.IsNullOrEmpty(queryParams.SearchTerm) || x.Name.ToLower().Contains(queryParams.SearchTerm.ToLower()))
    )
    {
        AddInclude(x => x.SubCategory);
        AddInclude(x => x.Brand);
        AddInclude(x => x.ProductDetails);
        if(queryParams.SortOrder == "asc")
        {
            AddOrderBy(x => x.Name);
        }
        else
        {
            AddOrderByDesc(x => x.Name);
        }
        if (queryParams.DoApplyPaging)
        {
            AddPaging(queryParams.Size * (queryParams.Page - 1), queryParams.Size);
        }
        else
        {
            queryParams.Size = -1;
            queryParams.Page = -1;
        }
    }
}
