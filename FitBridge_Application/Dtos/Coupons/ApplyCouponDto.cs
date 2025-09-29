namespace FitBridge_Application.Dtos.Coupons
{
    public class ApplyCouponDto
    {
        public Guid CouponId { get; set; }

        public decimal DiscountAmount { get; set; }

        public double DiscountPercent { get; set; }
    }
}