using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Specifications.Dashboards.GetAvailableBalanceDetail;
using MediatR;

namespace FitBridge_Application.Features.Dashboards.GetAvailableBalanceDetail
{
    public class GetAvailableBalanceDetailQuery(GetAvailableBalanceDetailParams parameters) : IRequest<PagingResultDto<AvailableBalanceTransactionDto>>
    {
        public GetAvailableBalanceDetailParams Params { get; set; } = parameters;
    }
}