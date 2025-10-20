using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Orders.GetOrderByCouponAndUserId
{
    public class GetOrderByCouponAndUserIdSpecification : BaseSpecification<Order>
    {
        public GetOrderByCouponAndUserIdSpecification(Guid CouponId, Guid userId) : base(x =>
            x.IsEnabled && x.CouponId == CouponId && x.AccountId == userId
            && x.Transactions.Any(t => t.Status == TransactionStatus.Success))
        {
            AddInclude(x => x.Coupon);
        }
    }
}