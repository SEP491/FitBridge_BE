namespace FitBridge_Application.Dtos.Coupons
{
    public class GetCouponsDto
    {
        public Guid Id { get; set; }

        public decimal MaxDiscount { get; set; }

        public string CouponCode { get; set; }

        public int NumberOfUsedCoupon { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }

        public DateOnly ExpirationDate { get; set; }

        public bool IsActive { get; set; }
    }
}