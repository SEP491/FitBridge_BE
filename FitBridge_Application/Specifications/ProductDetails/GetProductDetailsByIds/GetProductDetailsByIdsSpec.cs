using FitBridge_Domain.Entities.Ecommerce;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.ProductDetails.GetProductDetailsByIds
{
    public class GetProductDetailsByIdsSpec : BaseSpecification<ProductDetail>
    {
        public GetProductDetailsByIdsSpec(IEnumerable<Guid> productIds)
            : base(x =>
            x.IsEnabled
            && productIds.Contains(x.ProductId))
        {
        }
    }
}