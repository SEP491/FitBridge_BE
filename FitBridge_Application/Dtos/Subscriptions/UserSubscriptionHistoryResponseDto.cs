using System;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Dtos.Subscriptions;

public class UserSubscriptionHistoryResponseDto
{
    public Guid Id { get; set; }
    public string PlanName { get; set; }
    public decimal PlanCharge { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? UserSubscriptionLimitUsage { get; set; }
    public Guid FeatureKeyId { get; set; }
    public string FeatureKeyName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CurrentUsage { get; set; }
    public SubScriptionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
