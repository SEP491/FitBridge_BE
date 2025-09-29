using MediatR;

namespace FitBridge_Application.Features.Coupons.RemoveCoupon
{
    public class RemoveCouponCommand : IRequest
    {
        public Guid CouponId { get; set; }
    }
}