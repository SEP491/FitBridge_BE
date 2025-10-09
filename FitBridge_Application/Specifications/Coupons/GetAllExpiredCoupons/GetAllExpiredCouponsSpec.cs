using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Coupons.GetAllExpiredCoupons
{
    public class GetAllExpiredCouponsSpec : BaseSpecification<Coupon>
    {
        public GetAllExpiredCouponsSpec() : base(x =>
            x.IsEnabled && x.IsActive && x.ExpirationDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
        }
    }
}