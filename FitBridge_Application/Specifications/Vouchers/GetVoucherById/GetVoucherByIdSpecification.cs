using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Vouchers.GetVoucherById;

public class GetVoucherByIdSpecification : BaseSpecification<Coupon>
{
    public GetVoucherByIdSpecification(Guid voucherId) : base(x =>
       x.IsEnabled && x.Id == voucherId)
    {
    }
}