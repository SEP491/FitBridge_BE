using System;
using AutoMapper;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.GymCoursePts;

namespace FitBridge_Application.MappingProfiles;

public class GymCoursePtMappingProfile : Profile
{
    public GymCoursePtMappingProfile()
    {
        CreateProjection<GymCoursePT, GymCoursePtDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.GymCourseId, opt => opt.MapFrom(src => src.GymCourseId))
            .ForMember(dest => dest.PTId, opt => opt.MapFrom(src => src.PTId))
            .ForMember(dest => dest.Session, opt => opt.MapFrom(src => src.Session))
            .ForMember(dest => dest.PtImageUrl, opt => opt.MapFrom(src => src.PT.AvatarUrl))
            .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PT.FullName));
    }
}
