using FitBridge_Application.Commons.Utils;
using FitBridge_Application.Specifications.Gym.GetGymCoursesByGymId;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.Gym.GetGymPackages
{
    public class GetGymCoursesByGymIdSpecification : BaseSpecification<GymCourse>
    {
        public GetGymCoursesByGymIdSpecification(Guid gymId, GetGymCourseByGymIdParams parameters) : base(x => x.IsEnabled && x.GymOwnerId == gymId)
        {
            switch (StringCapitalizationConverter.ToUpperFirstChar(parameters.SortBy))
            {
                case nameof(GymCourse.Price):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.Price!);
                    else
                        AddOrderByDesc(x => x.Price!);
                    break;

                case nameof(GymCourse.Duration):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.Duration!);
                    else
                        AddOrderByDesc(x => x.Duration!);
                    break;

                default:
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.Id!);
                    else
                        AddOrderByDesc(x => x.Id!);
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
        }
    }
}