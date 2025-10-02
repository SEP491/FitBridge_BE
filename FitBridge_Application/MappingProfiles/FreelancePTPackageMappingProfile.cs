using AutoMapper;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles
{
    internal class FreelancePTPackageMappingProfile : Profile
    {
        protected FreelancePTPackageMappingProfile()
        {
            CreateProjection<FreelancePTPackage, GetAllFreelancePTPackagesDto>();
            CreateProjection<FreelancePTPackage, GetFreelancePTPackageByIdDto>();
        }
    }
}