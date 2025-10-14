using System;
using System.Drawing;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetAllFreelancePts;

public class GetAllFreelancePtsSpec : BaseSpecification<ApplicationUser> 
{
    public GetAllFreelancePtsSpec(GetAllFreelancePTsParam queryParams, List<Guid> userIds) : base(x => userIds.Contains(x.Id))
    {
        AddInclude(x => x.UserDetail);
        AddInclude(x => x.GoalTrainings);
        AddInclude(x => x.FreelancePtReviews);
        AddInclude(x => x.PTFreelancePackages);
        if (queryParams.DoApplyPaging)
        {
            AddPaging(queryParams.Size * (queryParams.Page - 1), queryParams.Size);
        }
        else
        {
            queryParams.Size = -1;
            queryParams.Page = -1;
        }
    }
}
