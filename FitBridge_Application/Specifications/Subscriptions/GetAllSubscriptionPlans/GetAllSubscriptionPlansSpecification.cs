using System;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.Subscriptions.GetAllSubscriptionPlans;

public class GetAllSubscriptionPlansSpecification : BaseSpecification<SubscriptionPlansInformation>
{
    public GetAllSubscriptionPlansSpecification(bool IsGetHotResearchSubscription) : base(x => x.IsEnabled && (IsGetHotResearchSubscription ? x.FeatureKey.FeatureName == ProjectConstant.FeatureKeyNames.HotResearch : true))
    {
        AddInclude(x => x.UserSubscriptions);
        AddInclude(x => x.FeatureKey);
        AddOrderBy(x => x.PlanName);
    }

}
