using AutoMapper;
using FitBridge_Application.Dtos.Accounts.Search;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.Gyms;
using System.Text;

namespace FitBridge_Application.MappingProfiles
{
    public class GymMappingProfiles : Profile
    {
        public GymMappingProfiles()
        {
            CreateProjection<ApplicationUser, GetGymDetailsDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(
                    src => new DateOnly(src.Dob.Year, src.Dob.Month, src.Dob.Day)))
                .ForMember(dest => dest.GymImages, opt => opt.MapFrom(
                    src => src.GymImages.Select(gi => new GymImageDto { Url = gi }).ToList()))
                .ForMember(dest => dest.RepresentName, opt => opt.MapFrom(
                    src => src.FullName));

            CreateProjection<ApplicationUser, GetAllGymsDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(
                    src => new DateOnly(src.Dob.Year, src.Dob.Month, src.Dob.Day)))
                .ForMember(dest => dest.GymImages, opt => opt.MapFrom(
                    src => src.GymImages.Select(gi => new GymImageDto { Url = gi }).ToList()))
                .ForMember(dest => dest.RepresentName, opt => opt.MapFrom(
                    src => src.FullName));

            CreateProjection<ApplicationUser, GetGymPtsDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(
                    src => new DateOnly(src.Dob.Year, src.Dob.Month, src.Dob.Day)))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(
                    src => src.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(
                    src => src.IsMale ? "Male" : "Female"))
                .ForMember(dest => dest.GoalTraining, opt => opt.MapFrom(
                    src => string.Join(", ", src.GoalTrainings.Select(x => x.Name))))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(
                    src => src.UserDetail!.Height))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(
                    src => src.UserDetail!.Weight))
                .ForMember(dest => dest.Experience, opt => opt.MapFrom(
                    src => src.UserDetail!.Experience));

            CreateMap<ApplicationUser, GetAllGymsForSearchDto>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(
                    src => new DateOnly(src.Dob.Year, src.Dob.Month, src.Dob.Day)))
                .ForMember(dest => dest.GymImages, opt => opt.MapFrom(
                    src => src.GymImages.Select(gi => new GymImageDto { Url = gi }).ToList()))
                .ForMember(dest => dest.RepresentName, opt => opt.MapFrom(
                    src => src.FullName));

            CreateMap<ApplicationUser, GetAllGymOwnerCustomer>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.IsMale ? "Male" : "Female"));

            CreateMap<ApplicationUser, GetGymOwnerCustomerDetail>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.IsMale ? "Male" : "Female"));
        }
    }
}