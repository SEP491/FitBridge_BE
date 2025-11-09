using System;

namespace FitBridge_Domain.Entities.ServicePackages;

public class FeatureKey : BaseEntity
{
    public string FeatureName { get; set; }
    public ICollection<SubscriptionPlansInformation> SubscriptionPlansInformation { get; set; } = new List<SubscriptionPlansInformation>();
}
