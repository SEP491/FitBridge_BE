using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Coupons.GetAllStartingCoupons
{
    public class GetAllStartingCouponsSpec : BaseSpecification<Coupon>
    {
        public GetAllStartingCouponsSpec() : base(x =>
            x.IsEnabled && x.IsActive && x.StartDate == DateOnly.FromDateTime(DateTime.UtcNow))
        {
        }
    }
}