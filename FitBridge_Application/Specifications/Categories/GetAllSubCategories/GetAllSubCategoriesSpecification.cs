using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Specifications.Categories.GetAllSubCategories;

public class GetAllSubCategoriesSpecification : BaseSpecification<SubCategory>
{
    public GetAllSubCategoriesSpecification() : base(x => x.IsEnabled)
    {
        AddOrderBy(x => x.Name);
    }
}
