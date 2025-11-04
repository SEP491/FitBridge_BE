using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.Subscriptions.GetAllSubscriptionPlans;

public class GetAllSubscriptionPlansSpecification : BaseSpecification<SubscriptionPlansInformation>
{
    public GetAllSubscriptionPlansSpecification() : base(x => x.IsEnabled )
    {
        AddInclude(x => x.UserSubscriptions);
        AddInclude(x => x.FeatureKey);
        AddOrderBy(x => x.PlanName);
    }

}
