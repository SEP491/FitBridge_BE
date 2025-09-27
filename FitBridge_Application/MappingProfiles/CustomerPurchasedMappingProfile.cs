using System;
using AutoMapper;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.GymCourses;

namespace FitBridge_Application.MappingProfiles;

public class CustomerPurchasedMappingProfile : Profile
{
    public CustomerPurchasedMappingProfile()
    {
        CreateProjection<CustomerPurchased, GymCoursesPtResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId))
            .ForMember(dest => dest.GymOwnerId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.GymOwnerId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Price))
            .ForMember(dest => dest.GymPtId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPtId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.OrderItems.First().GymCourse.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.ImageUrl))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.GymPt, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPt));
    }
}
