using System;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.Subscriptions.GetUserSubscriptionHistory;

public class GetUserSubscriptionHistorySpec : BaseSpecification<UserSubscription>
{
    public GetUserSubscriptionHistorySpec(GetUserSubscriptionHistoryParams parameters, Guid userId) : base(x => x.IsEnabled && x.UserId == userId)
    {
        AddOrderByDesc(x => x.CreatedAt);
        AddInclude(x => x.SubscriptionPlansInformation);
        AddInclude(x => x.SubscriptionPlansInformation.FeatureKey);
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
