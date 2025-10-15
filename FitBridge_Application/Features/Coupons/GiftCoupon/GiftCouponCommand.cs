using MediatR;

namespace FitBridge_Application.Features.Coupons.GiftCoupon
{
    public class GiftCouponCommand : IRequest
    {
        public List<Guid> CustomerIds { get; set; }

        public string CouponCode { get; set; }
    }
}