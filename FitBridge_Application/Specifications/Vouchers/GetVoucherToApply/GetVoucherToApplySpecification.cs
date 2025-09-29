using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Vouchers.GetVoucherToApply
{
    public class GetVoucherToApplySpecification : BaseSpecification<Coupon>
    {
        public GetVoucherToApplySpecification(Guid voucherId) : base(x => x.IsActive && x.IsEnabled && x.Id == voucherId)
        {
        }
    }
}