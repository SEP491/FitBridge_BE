using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetUsersByIds
{
    public class GetUsersByIdsSpec : BaseSpecification<ApplicationUser>
    {
        public GetUsersByIdsSpec(
            List<Guid> userIds,
            bool isIncludeBanned = false) : base(x =>
             x.IsEnabled
            && (isIncludeBanned || x.IsActive)
            && userIds.Contains(x.Id))
        {
        }
    }
}