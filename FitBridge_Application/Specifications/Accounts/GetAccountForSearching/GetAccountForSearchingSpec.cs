using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetAccountForSearching;

public class GetAccountForSearchingSpec : BaseSpecification<ApplicationUser>
{
    public GetAccountForSearchingSpec(GetAccountForSearchingParams parameters, List<Guid> userIds) : base(x => (((parameters.SearchTerm == null || parameters.SearchTerm.Trim() == "")
    || x.FullName.ToLower().Contains(parameters.SearchTerm.ToLower())
    || (x.GymName != null && x.GymName.ToLower().Contains(parameters.SearchTerm.ToLower()))
    || x.GoalTrainings.Any(gt => gt.Name.ToLower().Contains(parameters.SearchTerm.ToLower())))
    && (parameters.ExperienceYears <= 0 || (x.UserDetail != null && x.UserDetail.Experience >= parameters.ExperienceYears))
    && (parameters.Rating <= 0 || (x.Reviews.Any() && x.Reviews.Average(r => r.Rating) >= parameters.Rating))
    && (parameters.GoalTrainings == null
    || parameters.GoalTrainings.Count == 0
    || x.GoalTrainings.Any(gt => parameters.GoalTrainings.Contains(gt.Name)))
    && ((parameters.FromPrice <= 0 && parameters.ToPrice <= 0) // No price filter
        || (parameters.FromPrice > 0 && parameters.ToPrice <= 0 && (x.PTFreelancePackages.Any(p => p.Price >= parameters.FromPrice) || x.GymCourses.Any(gc => gc.Price >= parameters.FromPrice))) // Only minimum price
        || (parameters.FromPrice <= 0 && parameters.ToPrice > 0 && (x.PTFreelancePackages.Any(p => p.Price <= parameters.ToPrice) || x.GymCourses.Any(gc => gc.Price <= parameters.ToPrice))) // Only maximum price
        || (parameters.FromPrice > 0 && parameters.ToPrice > 0 && (x.PTFreelancePackages.Any(p => p.Price >= parameters.FromPrice && p.Price <= parameters.ToPrice) || x.GymCourses.Any(gc => gc.Price >= parameters.FromPrice && gc.Price <= parameters.ToPrice)))) // Price range
    )
    && (userIds == null || userIds.Count == 0 || userIds.Contains(x.Id))
    )
    {
        AddInclude(x => x.UserDetail);
        AddInclude(x => x.GoalTrainings);
        AddInclude(x => x.Reviews);
        AddInclude(x => x.GymFacilities);
        AddInclude(x => x.GymCourses);
        AddInclude(x => x.PTFreelancePackages);
        AddInclude(x => x.FreelancePtReviews);
        AddInclude(x => x.GymReviews);
        AddInclude(x => x.PTFreelancePackages);
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
