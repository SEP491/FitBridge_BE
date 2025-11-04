using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.Memberships.GetMembershipById;

public class GetMembershipByIdSpecification : BaseSpecification<SubscriptionPlansInformation>
{
    public GetMembershipByIdSpecification(Guid subscriptionPlansInformationId) : base(x =>
        x.IsEnabled && x.Id == subscriptionPlansInformationId)
    {
    }
}