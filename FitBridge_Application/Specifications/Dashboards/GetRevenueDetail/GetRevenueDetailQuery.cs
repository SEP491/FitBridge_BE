using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Dashboards;
using MediatR;

namespace FitBridge_Application.Specifications.Dashboards.GetRevenueDetail
{
    public class GetRevenueDetailQuery(GetRevenueDetailParams parameters) : IRequest<PagingResultDto<RevenueOrderItemDto>>
    {
        public GetRevenueDetailParams Params { get; set; } = parameters;
    }
}
