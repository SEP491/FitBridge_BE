using AutoMapper;
using FitBridge_Application.Dtos.Reports;
using FitBridge_Domain.Entities.Reports;

namespace FitBridge_Application.MappingProfiles
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {
     CreateProjection<ReportCases, GetCustomerReportsResponseDto>()
    .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.FullName))
       .ForMember(dest => dest.ReporterAvatarUrl, opt => opt.MapFrom(src => src.Reporter.AvatarUrl))
     .ForMember(dest => dest.ReportedUserName, opt => opt.MapFrom(src => src.ReportedUser != null ? src.ReportedUser.FullName : null))
     .ForMember(dest => dest.ReportedUserAvatarUrl, opt => opt.MapFrom(src => src.ReportedUser != null ? src.ReportedUser.AvatarUrl : null));

            CreateMap<ReportCases, GetCustomerReportsResponseDto>()
          .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.FullName))
   .ForMember(dest => dest.ReporterAvatarUrl, opt => opt.MapFrom(src => src.Reporter.AvatarUrl))
       .ForMember(dest => dest.ReportedUserName, opt => opt.MapFrom(src => src.ReportedUser != null ? src.ReportedUser.FullName : null))
                .ForMember(dest => dest.ReportedUserAvatarUrl, opt => opt.MapFrom(src => src.ReportedUser != null ? src.ReportedUser.AvatarUrl : null));
        }
  }
}
