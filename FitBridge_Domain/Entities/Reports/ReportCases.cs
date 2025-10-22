using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Reports;

namespace FitBridge_Domain.Entities.Reports;

public class ReportCases : BaseEntity
{
    public Guid ReporterId { get; set; }

    public ApplicationUser Reporter { get; set; }

    public Guid? ReportedUserId { get; set; }

    public ApplicationUser? ReportedUser { get; set; }

    public Guid OrderItemId { get; set; }

    public OrderItem OrderItem { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public List<string>? ImageUrls { get; set; }

    public ReportCaseStatus Status { get; set; }

    public string? Note { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public bool IsPayoutPaused { get; set; }

    public ReportCaseType ReportType { get; set; }
}