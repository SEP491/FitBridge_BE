using System;
using AutoMapper;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Dtos.Accounts.HotResearch;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.MappingProfiles;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<ApplicationUser, GetAllFreelancePTsResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.GoalTrainings, opt => opt.MapFrom(src => src.GoalTrainings.Select(x => x.Name).ToList()))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Bio))
            .ForMember(dest => dest.PriceFrom, opt => opt.MapFrom(src => src.PTFreelancePackages.Min(x => x.Price)))
            .ForMember(dest => dest.ExperienceYears, opt => opt.MapFrom(src => src.UserDetail.Experience))
            .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.UserDetail.Certificates))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Reviews.Count > 0 ? src.Reviews.Average(x => x.Rating) : 0));

        CreateMap<ApplicationUser, GetFreelancePtByIdResponseDto>()
            .ForMember(dest => dest.FreelancePt, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.UserDetail, opt => opt.MapFrom(src => src.UserDetail))
            .ForMember(dest => dest.FreelancePTPackages, opt => opt.MapFrom(src => src.PTFreelancePackages));

        CreateMap<ApplicationUser, HotResearchAccountDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.HotResearch, opt => opt.MapFrom(src => src.hotResearch))
            .ForMember(dest => dest.GymName, opt => opt.MapFrom(src => src.GymName));
    }
}
