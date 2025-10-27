using FitBridge_Domain.Enums.Reports;

namespace FitBridge_Application.Dtos.Reports
{
    public class GetCustomerReportsResponseDto
    {
        public Guid Id { get; set; }

        public string ReporterName { get; set; } = string.Empty;

        public string? ReporterAvatarUrl { get; set; }

        public string? ReportedUserName { get; set; }

        public string? ReportedUserAvatarUrl { get; set; }

        public Guid OrderItemId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ReportCaseStatus Status { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public ReportCaseType ReportType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}