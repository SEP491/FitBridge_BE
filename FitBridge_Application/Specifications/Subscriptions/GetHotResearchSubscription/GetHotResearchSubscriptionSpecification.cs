using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Application.Commons.Constants;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Specifications.Subscriptions.GetHotResearchSubscription;

public class GetHotResearchSubscriptionSpecification : BaseSpecification<UserSubscription>
{
    public GetHotResearchSubscriptionSpecification() : base(x => x.SubscriptionPlansInformation.FeatureKey.FeatureName == ProjectConstant.FeatureKeyNames.HotResearch && x.Status == SubScriptionStatus.Active)
    {
    }
}
