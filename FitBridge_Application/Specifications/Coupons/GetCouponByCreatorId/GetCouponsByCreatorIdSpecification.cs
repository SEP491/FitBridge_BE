using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Coupons.GetCouponByCreatorId
{
    public class GetCouponsByCreatorIdSpecification : BaseSpecification<Coupon>
    {
        public GetCouponsByCreatorIdSpecification(GetCouponsByCreatorIdParam param, Guid creatorId) : base(x =>
            x.IsEnabled && x.CreatorId == creatorId)
        {
            if (param.DoApplyPaging)
            {
                AddPaging(param.Size * (param.Page - 1), param.Size);
            }
        }
    }
}