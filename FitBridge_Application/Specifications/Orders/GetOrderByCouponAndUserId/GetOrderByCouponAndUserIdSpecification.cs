using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId
{
    public class GetOrderByCouponAndUserIdSpecification : BaseSpecification<Order>
    {
        public GetOrderByCouponAndUserIdSpecification(Guid CouponId, Guid userId) : base(x =>
            x.IsEnabled && x.CouponId == CouponId && x.AccountId == userId)
        {
            AddInclude(x => x.Coupon);
        }
    }
}