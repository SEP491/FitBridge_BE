using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Dtos.OrderItems;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Coupons.ApplyCoupon
{
    public class ApplyCouponQuery : IRequest<ApplyCouponDto>
    {
        [JsonIgnore]
        public Guid CouponId { get; set; }

        public List<OrderItemDto> OrderItemDtos { get; set; }
    }
}