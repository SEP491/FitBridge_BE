using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.Memberships.GetMembershipById;

public class GetMembershipByIdSpecification : BaseSpecification<ServiceInformation>
{
    public GetMembershipByIdSpecification(Guid membershipId) : base(x =>
        x.IsEnabled && x.Id == membershipId)
    {
    }
}
