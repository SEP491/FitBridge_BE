using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Specifications.Subscriptions.GetTempSubscription;

public class GetTempSubscriptionSpec : BaseSpecification<UserSubscription>
{
    public GetTempSubscriptionSpec(Guid orderItemId) : base(x => x.OrderItem.Id == orderItemId && x.Status == SubScriptionStatus.Created)
    {
        AddInclude(x => x.OrderItem);
    }
}
