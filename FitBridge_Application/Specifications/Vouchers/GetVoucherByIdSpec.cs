using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Vouchers;

public class GetVoucherByIdSpec : BaseSpecification<Voucher>
{
    public GetVoucherByIdSpec(Guid voucherId) : base(x => x.Id == voucherId)
    {
    }
}
