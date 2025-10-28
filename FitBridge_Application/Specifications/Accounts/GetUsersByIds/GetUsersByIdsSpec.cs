using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetUsersByIds
{
    public class GetUsersByIdsSpec : BaseSpecification<ApplicationUser>
    {
        public GetUsersByIdsSpec(
            List<Guid> userIds) : base(x =>
             x.IsEnabled && x.IsActive && userIds.Contains(x.Id))
        {
        }
    }
}