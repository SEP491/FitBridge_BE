using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Accounts.GetAllGymPts
{
    public class GetAllGymPtsSpec : BaseSpecification<ApplicationUser>
    {
        public GetAllGymPtsSpec(
            List<Guid> userIds,
            Guid gymOwnerId,
            GetAllGymPtsParams? queryParams = null) : base(x =>
            userIds.Contains(x.Id)
            && x.GymOwnerId == gymOwnerId)
        {
            AddInclude(x => x.UserDetail);
            AddInclude(x => x.GoalTrainings);
            AddInclude(x => x.GymReviews);
            AddInclude(x => x.GymOwner);
            AddInclude(x => x.GymCoursePTs);

            if (queryParams is null)
            {
                return;
            }
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
}