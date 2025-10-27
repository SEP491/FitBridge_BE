using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Application.Specifications.Reports.GetAllReports;
using MediatR;

namespace FitBridge_Application.Features.Reports.GetAllReports
{
    public class GetAllReportsQuery : IRequest<PagingResultDto<GetCustomerReportsResponseDto>>
    {
        public GetAllReportsParams Params { get; set; } = null!;
    }
}
