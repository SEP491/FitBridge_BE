using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Specifications.Dashboards.GetOrderItemForRevenueDetail;
using MediatR;

namespace FitBridge_Application.Features.Dashboards.GetRevenueDetail
{
    public class GetRevenueDetailQuery(GetRevenueDetailParams parameters) : IRequest<DashboardPagingResultDto<RevenueOrderItemDto>>
    {
        public GetRevenueDetailParams Params { get; set; } = parameters;
    }
}