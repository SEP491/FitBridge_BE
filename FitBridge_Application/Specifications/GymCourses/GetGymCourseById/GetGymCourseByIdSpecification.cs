using FitBridge_Domain.Entities.Gyms;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.GymCourses.GetGymCourseById
{
    public class GetGymCourseByIdSpecification : BaseSpecification<GymCourse>
    {
        public GetGymCourseByIdSpecification(
            Guid gymCourseId,
            bool includeGymOwner = true) : base(x =>
            x.Id == gymCourseId &&
            x.IsEnabled)
        {
            if (includeGymOwner)
            {
                AddInclude(x => x.GymOwner);
            }
        }
    }
}