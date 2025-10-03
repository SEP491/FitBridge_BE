using AutoMapper;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles
{
    public class FreelancePTPackageMappingProfile : Profile
    {
        public FreelancePTPackageMappingProfile()
        {
            CreateProjection<FreelancePTPackage, GetAllFreelancePTPackagesDto>();
            CreateProjection<FreelancePTPackage, GetFreelancePTPackageByIdDto>();
            CreateMap<FreelancePTPackage, CreateFreelancePTPackageDto>();
        }
    }
}