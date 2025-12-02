using FitBridge_Domain.Entities.Ecommerce;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Categories.GetCategoryByName
{
    public class GetCategoryByNameSpec : BaseSpecification<Category>
    {
        public GetCategoryByNameSpec(string name) : base(x => x.Name == name)
        {
        }
    }
}
