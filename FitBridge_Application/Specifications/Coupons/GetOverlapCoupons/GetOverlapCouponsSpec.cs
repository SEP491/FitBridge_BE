﻿using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Coupons.GetOverlapCoupons
{
    public class GetOverlapCouponsSpec : BaseSpecification<Coupon>
    {
        public GetOverlapCouponsSpec(DateOnly startDate) : base(x =>
            x.IsActive && x.IsEnabled && x.NumberOfUsedCoupon != 0
            && x.ExpirationDate >= startDate)
        {
        }
    }
}