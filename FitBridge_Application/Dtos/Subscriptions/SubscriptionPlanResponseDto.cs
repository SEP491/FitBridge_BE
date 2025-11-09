using System;

namespace FitBridge_Application.Dtos.Subscriptions;

public class SubscriptionPlanResponseDto
{
    public Guid Id { get; set; }
    public string PlanName { get; set; }
    public decimal PlanCharge { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? LimitUsage { get; set; }
    public Guid FeatureKeyId { get; set; }
    public string FeatureKeyName { get; set; }
    public bool IsActiveForCurrentUser { get; set; }
    public CurrentUserSubscriptionResponseDto? CurrentUserSubscription { get; set; }
}

