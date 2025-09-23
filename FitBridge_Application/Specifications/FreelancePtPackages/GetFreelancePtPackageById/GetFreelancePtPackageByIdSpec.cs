using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.Specifications.FreelancePtPackages.GetFreelancePtPackageById;

public class GetFreelancePtPackageByIdSpec : BaseSpecification<FreelancePTPackage>
{
    public GetFreelancePtPackageByIdSpec(Guid freelancePTPackageId) : base(x => x.Id == freelancePTPackageId)
    {
    }
}
