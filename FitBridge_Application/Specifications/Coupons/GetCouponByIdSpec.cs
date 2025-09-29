using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Coupons;

public class GetCouponByIdSpec : BaseSpecification<Coupon>
{
    public GetCouponByIdSpec(Guid couponId) : base(x => x.Id == couponId)
    {
    }
}
