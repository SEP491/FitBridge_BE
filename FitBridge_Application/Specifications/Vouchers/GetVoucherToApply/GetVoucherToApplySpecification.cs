using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Vouchers.GetVoucherToApply
{
    public class GetVoucherToApplySpecification : BaseSpecification<Voucher>
    {
        public GetVoucherToApplySpecification(Guid voucherId) : base(x => x.IsActive && x.IsEnabled && x.Id == voucherId)
        {
        }
    }
}