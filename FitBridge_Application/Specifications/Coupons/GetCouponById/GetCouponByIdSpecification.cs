using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Coupons.GetCouponById;

public class GetCouponByIdSpecification : BaseSpecification<Coupon>
{
    public GetCouponByIdSpecification(Guid CouponId, bool getActiveOnly = false) : base(x =>
       x.IsEnabled && x.Id == CouponId && (!getActiveOnly || (getActiveOnly && x.IsActive)))
    {
    }
}