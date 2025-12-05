using FitBridge_Domain.Entities.Gyms;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.FreelancePtPackages.GetAllFreelancePTPackages
{
    public class GetAllFreelancePTPackagesSpec : BaseSpecification<FreelancePTPackage>
    {
        public GetAllFreelancePTPackagesSpec(GetAllFreelancePTPackagesParam parameters, Guid ptId) : base(x =>
            x.IsEnabled && x.PtId == ptId &&
            (string.IsNullOrEmpty(parameters.SearchTerm) || parameters.SearchTerm.ToLower().Contains(x.Name)))
        {
            AddInclude(x => x.Pt);
            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size - 1);
            }
        }
    }
}