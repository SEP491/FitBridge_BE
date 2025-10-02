using FitBridge_Domain.Entities.Gyms;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.FreelancePtPackages.GetAllFreelancePTPackages
{
    public class GetAllFreelancePTPackagesSpec : BaseSpecification<FreelancePTPackage>
    {
        public GetAllFreelancePTPackagesSpec(GetAllFreelancePTPackagesParam parameters) : base(x =>
            x.IsEnabled &&
            (string.IsNullOrEmpty(parameters.SearchTerm) || parameters.SearchTerm.ToLower().Contains(x.Name)))
        {
            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size - 1);
            }
        }
    }
}