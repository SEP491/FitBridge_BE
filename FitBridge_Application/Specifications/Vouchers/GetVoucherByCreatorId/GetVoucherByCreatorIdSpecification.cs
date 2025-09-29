using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Vouchers.GetVoucherByCreatorId
{
    public class GetVoucherByCreatorIdSpecification : BaseSpecification<Coupon>
    {
        public GetVoucherByCreatorIdSpecification(GetVouchersByCreatorIdParam param, Guid creatorId) : base(x =>
            x.IsEnabled && x.CreatorId == creatorId)
        {
            if (param.DoApplyPaging)
            {
                AddPaging(param.Size * (param.Page - 1), param.Size);
            }
        }
    }
}