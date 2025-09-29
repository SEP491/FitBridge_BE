using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Coupons;
using FitBridge_Application.Specifications.Vouchers.GetVoucherByCreatorId;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.GetUserVouchers
{
    public class GetUserCreatedVouchersQuery : IRequest<PagingResultDto<GetCouponsDto>>
    {
        public GetVouchersByCreatorIdParam Params { get; set; }
    }
}