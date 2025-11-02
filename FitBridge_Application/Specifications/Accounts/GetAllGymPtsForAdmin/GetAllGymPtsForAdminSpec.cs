using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetAllGymPtsForAdmin;

public class GetAllGymPtsForAdminSpec : BaseSpecification<ApplicationUser>
{
    public GetAllGymPtsForAdminSpec(GetAllGymPtsForAdminParams parameters, List<Guid> userIds) : base(x => userIds.Contains(x.Id) && x.IsEnabled)
    {
        AddInclude(x => x.UserDetail);
        AddInclude(x => x.GoalTrainings);
        AddInclude(x => x.GymReviews);
        AddInclude(x => x.GymOwner);
        if (parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
