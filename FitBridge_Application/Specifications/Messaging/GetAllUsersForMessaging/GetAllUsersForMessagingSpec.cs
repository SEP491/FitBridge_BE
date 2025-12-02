using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Messaging.GetAllUsersForMessaging;

public class GetAllUsersForMessagingSpec : BaseSpecification<ApplicationUser>
{
    public GetAllUsersForMessagingSpec(GetAllUsersForMessagingParam parameter, Guid currentUserId)
        : base(u =>
            u.Id != currentUserId &&
            (string.IsNullOrEmpty(parameter.SearchTerm) ||
            u.FullName.ToLower().Contains(parameter.SearchTerm.ToLower()) ||
            (u.Email != null && u.Email.ToLower().Contains(parameter.SearchTerm.ToLower()))))
    {
        if (parameter.SortOrder.ToLower() == "desc")
        {
            AddOrderByDesc(u => u.FullName);
        }
        else
        {
            AddOrderBy(u => u.FullName);
        }

        if (parameter.DoApplyPaging)
        {
            AddPaging((parameter.Page - 1) * parameter.Size, parameter.Size);
        }
    }
}