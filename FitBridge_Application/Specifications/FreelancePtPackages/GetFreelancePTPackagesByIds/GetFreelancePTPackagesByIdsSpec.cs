using FitBridge_Domain.Entities.Gyms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePTPackagesByIds
{
    internal class GetFreelancePTPackagesByIdsSpec : BaseSpecification<FreelancePTPackage>
    {
        public GetFreelancePTPackagesByIdsSpec(IEnumerable<Guid> ids, Guid? creatorId = null)
            : base(x => x.IsEnabled
            && ids.Contains(x.Id)
            && (creatorId == null || creatorId == x.PtId))
        {
        }
    }
}