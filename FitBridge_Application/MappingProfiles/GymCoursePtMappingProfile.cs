using System;
using AutoMapper;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.GymCoursePts;
using FitBridge_Application.Dtos.GymCourses;

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
        CreateMap<GymCourse, GymCourseResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.GymOwnerId, opt => opt.MapFrom(src => src.GymOwnerId));
    }
}
