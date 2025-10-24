using FitBridge_Domain.Entities.Gyms;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.GymCourses.GetGymCourseByIds
{
    public class GetGymCourseByIdsSpec : BaseSpecification<GymCourse>
    {
        public GetGymCourseByIdsSpec(IEnumerable<Guid> ids, Guid? creatorId = null)
            : base(x => x.IsEnabled
            && ids.Contains(x.Id)
            && (creatorId == null || creatorId == x.GymOwnerId))
        {
        }
    }
}