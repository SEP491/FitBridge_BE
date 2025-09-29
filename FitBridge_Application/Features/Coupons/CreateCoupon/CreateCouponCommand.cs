using FitBridge_Application.Dtos.Coupons;
using MediatR;

namespace FitBridge_Application.Features.Coupons.CreateCoupon
{
    public class CreateCouponCommand : IRequest<CreateNewCouponDto>
    {
        public decimal MaxDiscount { get; set; }

        public double DiscountPercent { get; set; }

        public int Quantity { get; set; }
    }
}