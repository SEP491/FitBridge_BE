using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Specifications.Coupons.GetCouponByCreatorId;
using MediatR;

namespace FitBridge_Application.Features.Coupons.GetUserCreatedCoupons
{
    public class GetUserCreatedCouponsQuery : IRequest<PagingResultDto<GetCouponsDto>>
    {
        public GetCouponsByCreatorIdParam Params { get; set; }
    }
}