using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitBridge_Application.Dtos.Reports;
using MediatR;

namespace FitBridge_Application.Features.Reports.GetReportById
{
    public class GetReportByIdQuery : IRequest<GetCustomerReportsResponseDto>
    {
        public Guid ReportId { get; set; }
    }
}
