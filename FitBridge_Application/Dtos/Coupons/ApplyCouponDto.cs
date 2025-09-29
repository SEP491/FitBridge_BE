namespace FitBridge_Application.Dtos.Coupons
{
    public class ApplyCouponDto
    {
        public Guid VoucherId { get; set; }

        public decimal VoucherDiscount { get; set; }

        public double DiscountPercent { get; set; }
    }
}