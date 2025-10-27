using FitBridge_Domain.Enums.Reports;

namespace FitBridge_Application.Dtos.Reports
{
    public class GetCustomerReportsResponseDto
    {
        public Guid Id { get; set; }

        public Guid ReporterId { get; set; }

        public string ReporterName { get; set; } = string.Empty;

        public string? ReporterAvatarUrl { get; set; }

        public Guid? ReportedUserId { get; set; }

        public string? ReportedUserName { get; set; }

        public string? ReportedUserAvatarUrl { get; set; }

        public Guid OrderItemId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<string>? ImageUrls { get; set; }

        public ReportCaseStatus Status { get; set; }

        public string? Note { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public bool IsPayoutPaused { get; set; }

        public ReportCaseType ReportType { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}