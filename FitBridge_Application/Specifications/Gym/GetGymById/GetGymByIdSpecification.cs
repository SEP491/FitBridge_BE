using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Gym.GetGymById
{
    public class GetGymByIdSpecification : BaseSpecification<ApplicationUser>
    {
        public GetGymByIdSpecification(
            Guid gymId,
            bool includeGymFacilities = false,
            bool includeGymCoursePTs = false
            ) : base(x => x.Id == gymId)
        {
            if (includeGymFacilities)
            {
                AddInclude(x => x.GymFacilities);
            }
            if (includeGymCoursePTs)
            {
                AddInclude(x => x.GymCoursePTs);
            }
            AddInclude(x => x.GymAssets);
        }
    }
}