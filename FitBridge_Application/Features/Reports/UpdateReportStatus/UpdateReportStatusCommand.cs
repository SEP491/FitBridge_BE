using FitBridge_Domain.Enums.Reports;
using MediatR;

namespace FitBridge_Application.Features.Reports.UpdateReportStatus
{
    public class UpdateReportStatusCommand : IRequest
    {
        public Guid ReportId { get; set; }

        public string? Note { get; set; }

        public ReportCaseStatus Status { get; set; }
    }
}