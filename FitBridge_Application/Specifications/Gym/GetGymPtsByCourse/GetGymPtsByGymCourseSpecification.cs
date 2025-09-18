using FitBridge_Application.Commons.Utils;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Gym.GetGymPtsByCourse
{
    public class GetGymPtsByGymCourseSpecification : BaseSpecification<ApplicationUser>
    {
        public GetGymPtsByGymCourseSpecification(
            Guid courseId,
            GetGymPtsByGymCourseParams parameters,
            bool includeUserDetails = true,
            bool includeUserGoalTraining = true) : base(x =>
            x.GymCoursePTs.Where(x => x.GymCourseId == courseId).Select(x => x.PTId).Contains(x.Id))
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