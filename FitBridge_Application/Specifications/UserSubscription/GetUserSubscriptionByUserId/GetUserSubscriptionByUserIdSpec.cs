using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.ServicePackages;
using FitBridge_Domain.Enums.SubscriptionPlans;

namespace FitBridge_Application.Specifications.UserSubscriptions.GetUserSubscriptionByUserId;

public class GetUserSubscriptionByUserIdSpec : BaseSpecification<UserSubscription>
{
    public GetUserSubscriptionByUserIdSpec(Guid userId, Guid subscriptionPlanId) : base(x => x.UserId == userId && (x.Status == SubScriptionStatus.Active || x.Status == SubScriptionStatus.Created) && x.SubscriptionPlanId == subscriptionPlanId)
    {
    }
}
