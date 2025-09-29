using MediatR;

namespace FitBridge_Application.Features.Coupons.UpdateCoupon
{
    public class UpdateCouponCommand : IRequest
    {
        public Guid CouponId { get; set; }

        public decimal? MaxDiscount { get; set; }

        public double? DiscountPercent { get; set; }

        public int? Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}