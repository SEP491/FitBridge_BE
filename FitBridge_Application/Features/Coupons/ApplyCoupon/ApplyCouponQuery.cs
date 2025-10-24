using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Dtos.OrderItems;
using MediatR;

namespace FitBridge_Application.Features.Coupons.ApplyCoupon
{
    public class ApplyCouponQuery : IRequest<ApplyCouponDto>
    {
        public string CouponCode { get; set; }

        public string ProductType { get; set; }

        public List<Guid> ItemsId { get; set; } = new();

        public decimal TotalPrice { get; set; }
    }
}