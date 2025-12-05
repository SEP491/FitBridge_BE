using System;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Domain.Entities.ServicePackages;

public class UserSubscription : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? LimitUsage { get; set; }
    public int CurrentUsage { get; set; }
    public SubScriptionStatus Status { get; set; }
    public ApplicationUser User { get; set; }
    public SubscriptionPlansInformation SubscriptionPlansInformation { get; set; }
    public OrderItem OrderItem { get; set; }
}
