using FitBridge_Domain.Enums.Reports;
using MediatR;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Reports.UpdateReportStatus
{
    public class UpdateReportStatusCommand : IRequest
    {
        [JsonIgnore]
        public Guid ReportId { get; set; }

        public string? Note { get; set; } = null;

        public ReportCaseStatus Status { get; set; }
    }
}