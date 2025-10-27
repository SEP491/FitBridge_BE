using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Specifications.Reports.GetCustomerReports;
using MediatR;

namespace FitBridge_Application.Features.Reports.GetCustomerReports
{
    public class GetCustomerReportsQuery : IRequest<PagingResultDto<GetCustomerReportsResponseDto>>
    {
        public GetCustomerReportsParams Params { get; set; } = null!;
    }
}