using FitBridge_Application.Dtos.Coupons;
using MediatR;

namespace FitBridge_Application.Features.Coupons.GetCouponById
{
    public class GetCouponByIdQuery : IRequest<GetCouponsDto>
    {
        public Guid Id { get; set; }
    }
}