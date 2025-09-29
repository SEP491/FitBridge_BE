using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Coupons
{
    public class CreateNewCouponDto
    {
        public Guid Id { get; set; }

        public decimal MaxDiscount { get; set; }

        public CouponType Type { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }

        public Guid CreatorId { get; set; }
    }
}