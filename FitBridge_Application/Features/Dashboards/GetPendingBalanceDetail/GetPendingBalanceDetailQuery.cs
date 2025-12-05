using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Specifications.Dashboards.GetPendingBalanceDetail;
using MediatR;

namespace FitBridge_Application.Features.Dashboards.GetPendingBalanceDetail
{
    public class GetPendingBalanceDetailQuery(GetPendingBalanceDetailParams parameters) : IRequest<DashboardPagingResultDto<PendingBalanceOrderItemDto>>
    {
        public GetPendingBalanceDetailParams Params { get; set; } = parameters;
    }
}