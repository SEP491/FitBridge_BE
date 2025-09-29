using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Dtos.OrderItems;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Vouchers.ApplyCoupon
{
    public class ApplyCouponQuery : IRequest<ApplyCouponDto>
    {
        [JsonIgnore]
        public Guid VoucherId { get; set; }

        public List<OrderItemDto> OrderItemDtos { get; set; }
    }
}