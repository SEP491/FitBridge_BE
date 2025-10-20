using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Specifications.Payments.GetAllWithdrawalRequests;
using MediatR;

namespace FitBridge_Application.Features.Payments.GetAllWithdrawalRequests
{
    public class GetAllWithdrawalRequestsQuery : IRequest<PagingResultDto<GetWithdrawalRequestResponseDto>>
    {
        public GetAllWithdrawalRequestsParams Params { get; set; }
    }
}
