using System;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Dtos.Subscriptions;

public class CurrentUserSubscriptionResponseDto
{
    public Guid UserSubscriptionId { get; set; }
    public Guid UserId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? UserSubscriptionLimitUsage { get; set; }
    public int UserSubscriptionCurrentUsage { get; set; }
    public SubScriptionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
