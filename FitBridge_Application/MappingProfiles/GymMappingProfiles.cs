using AutoMapper;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.MappingProfiles
{
    public class GymMappingProfiles : Profile
    {
        public GymMappingProfiles()
        {
            CreateProjection<ApplicationUser, GetGymDetailsDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(
                    src => new DateOnly(src.Dob.Year, src.Dob.Month, src.Dob.Day)))
                .ForMember(dest => dest.RepresentName, opt => opt.MapFrom(
                    src => src.FullName));

            CreateProjection<ApplicationUser, GetAllGymsDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(
                    src => new DateOnly(src.Dob.Year, src.Dob.Month, src.Dob.Day)))
                .ForMember(dest => dest.RepresentName, opt => opt.MapFrom(
                    src => src.FullName));
        }
    }
}