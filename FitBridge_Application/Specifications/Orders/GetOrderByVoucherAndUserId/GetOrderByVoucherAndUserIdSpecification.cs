using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Orders.GetOrderByVoucherId
{
    public class GetOrderByVoucherAndUserIdSpecification : BaseSpecification<Order>
    {
        public GetOrderByVoucherAndUserIdSpecification(Guid voucherId, Guid userId) : base(x =>
            x.IsEnabled && x.CouponId == voucherId && x.AccountId == userId)
        {
            AddInclude(x => x.Coupon);
        }
    }
}