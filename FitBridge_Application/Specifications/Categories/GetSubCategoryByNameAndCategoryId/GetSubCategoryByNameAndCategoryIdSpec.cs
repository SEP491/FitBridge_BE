using FitBridge_Domain.Entities.Ecommerce;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Categories.GetSubCategoryByNameAndCategoryId
{
    public class GetSubCategoryByNameAndCategoryIdSpec : BaseSpecification<SubCategory>
    {
        public GetSubCategoryByNameAndCategoryIdSpec(string name, Guid categoryId) : base(x =>
            x.Name == name && x.CategoryId == categoryId)
        {
        }
    }
}
