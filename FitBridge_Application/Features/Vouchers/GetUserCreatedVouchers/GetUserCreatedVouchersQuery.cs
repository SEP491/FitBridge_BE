using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Vouchers;
using FitBridge_Application.Specifications.Vouchers.GetVoucherByCreatorId;
using MediatR;

namespace FitBridge_Application.Features.Vouchers.GetUserVouchers
{
    public class GetUserCreatedVouchersQuery : IRequest<PagingResultDto<GetVouchersDto>>
    {
        public GetVouchersByCreatorIdParam Params { get; set; }
    }
}