using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Specifications.Dashboards.GetTransactionForAvailableBalanceDetail;
using MediatR;

namespace FitBridge_Application.Features.Dashboards.GetAvailableBalanceDetail
{
    public class GetAvailableBalanceDetailQuery(GetAvailableBalanceDetailParams parameters) : IRequest<DashboardPagingResultDto<AvailableBalanceTransactionDto>>
    {
        public GetAvailableBalanceDetailParams Params { get; set; } = parameters;
    }
}