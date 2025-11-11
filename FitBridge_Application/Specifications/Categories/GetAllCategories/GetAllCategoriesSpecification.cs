using System;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Categories.GetAllCategories;

public class GetAllCategoriesSpecification : BaseSpecification<Category>
{
    public GetAllCategoriesSpecification() : base(x => x.IsEnabled)
    {
        AddOrderBy(x => x.Name);
    }
}
