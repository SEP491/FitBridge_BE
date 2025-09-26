using System;
using AutoMapper;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Dtos.GymPTs;

namespace FitBridge_Application.MappingProfiles;

public class GymPtMappingProfile : Profile
{
    public GymPtMappingProfile()
    {
        CreateProjection<ApplicationUser, GymPtResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => src.IsMale))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.UserDetail!.Experience));
    }
}
