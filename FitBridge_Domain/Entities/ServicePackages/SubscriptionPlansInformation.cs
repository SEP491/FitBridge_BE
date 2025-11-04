using System;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.ServicePackages;

public class SubscriptionPlansInformation : BaseEntity
{
    public string PlanName { get; set; }
    public decimal PlanCharge { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? LimitUsage { get; set; }
    public Guid FeatureKeyId { get; set; }
    public FeatureKey FeatureKey { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
