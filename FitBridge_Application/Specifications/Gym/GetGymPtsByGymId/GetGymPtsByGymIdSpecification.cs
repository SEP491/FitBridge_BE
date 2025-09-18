using FitBridge_Application.Commons.Utils;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Gym
{
    public class GetGymPtsByGymIdSpecification : BaseSpecification<ApplicationUser>
    {
        public GetGymPtsByGymIdSpecification(
            Guid gymId,
            GetGymPtsByGymIdParams parameters,
            bool includeUserDetails = true,
            bool includeUserGoalTraining = true) : base(x =>
            x.GymOwnerId == gymId && x.GymOwnerId != null)
        {
            switch (StringCapitalizationConverter.ToUpperFirstChar(parameters.SortBy))
            {
                default:
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.FullName!);
                    else
                        AddOrderByDesc(x => x.FullName!);
                    break;
            }

            if (parameters.DoApplyPaging)
            {
                AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
            }
            else
            {
                parameters.Size = -1;
                parameters.Page = -1;
            }

            if (includeUserDetails)
            {
                AddInclude(x => x.UserDetail);
            }

            if (includeUserGoalTraining)
            {
                AddInclude(x => x.GoalTrainings);
            }
        }
    }
}