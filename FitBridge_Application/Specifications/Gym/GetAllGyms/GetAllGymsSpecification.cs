using FitBridge_Application.Commons.Utils;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Domain.Entities.Identity;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Gym.GetAllGyms
{
    public class GetAllGymsSpecification : BaseSpecification<ApplicationUser>
    {
        public GetAllGymsSpecification(
            GetAllGymsParams parameters,
            bool includeGymFacilities = false,
            bool includeGymCoursePTs = false) : base(x =>
                (!string.IsNullOrEmpty(x.GymName) &&
                    (string.IsNullOrEmpty(parameters.SearchTerm) || x.GymName!.ToLower().Contains(parameters.SearchTerm.ToLower())))
            )
        {
            switch (StringCapitalizationConverter.ToUpperFirstChar(parameters.SortBy))
            {
                case nameof(GetAllGymsDto.GymName):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.GymName!);
                    else
                        AddOrderByDesc(x => x.GymName!);
                    break;

                case nameof(GetAllGymsDto.RepresentName):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.FullName!);
                    else
                        AddOrderByDesc(x => x.FullName!);
                    break;

                default:
                    AddOrderBy(x => x.GymName!);
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

            if (includeGymFacilities)
            {
                AddInclude(x => x.GymFacilities);
            }
            if (includeGymCoursePTs)
            {
                AddInclude(x => x.GymCoursePTs);
            }
        }
    }
}