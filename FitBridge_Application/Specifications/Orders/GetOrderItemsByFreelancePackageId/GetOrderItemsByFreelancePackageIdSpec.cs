using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Orders.GetOrderItemsByFreelancePackageId
{
    public class GetOrderItemsByFreelancePackageIdSpec : BaseSpecification<OrderItem>
    {
        public GetOrderItemsByFreelancePackageIdSpec(Guid freelancePackageId) : base(x => x.FreelancePTPackageId == freelancePackageId)
        {
        }
    }
}