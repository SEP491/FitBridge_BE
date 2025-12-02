using AutoMapper;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles
{
    public class FreelancePTPackageMappingProfile : Profile
    {
        public FreelancePTPackageMappingProfile()
        {
            CreateMap<FreelancePTPackage, GetAllFreelancePTPackagesDto>();
            CreateMap<FreelancePTPackage, GetFreelancePTPackageByIdDto>();
            CreateMap<FreelancePTPackage, CreateFreelancePTPackageDto>();
            CreateMap<FreelancePTPackage, GetFreelancePTPackageWithPt>()
            .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.Pt.FullName))
            .ForMember(dest => dest.PtImageUrl, opt => opt.MapFrom(src => src.Pt.AvatarUrl))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.DurationInDays))
            .ForMember(dest => dest.SessionDurationInMinutes, opt => opt.MapFrom(src => src.SessionDurationInMinutes))
            .ForMember(dest => dest.NumOfSessions, opt => opt.MapFrom(src => src.NumOfSessions))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
            .ForMember(dest => dest.PtImageUrl, opt => opt.MapFrom(src => src.Pt.AvatarUrl))
            .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.Pt.FullName));
        }
    }
}