using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Coupons.GetCouponToApply
{
    public class GetCouponToApplySpecification : BaseSpecification<Coupon>
    {
        public GetCouponToApplySpecification(Guid CouponId) : base(x => x.IsActive && x.IsEnabled && x.Id == CouponId)
        {
        }
    }
}