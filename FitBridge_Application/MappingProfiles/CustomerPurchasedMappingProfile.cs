using System;
using AutoMapper;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Dtos.CustomerPurchaseds;

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

        CreateProjection<CustomerPurchased, CustomerPurchasedResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Name))
            .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.ImageUrl))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.CanAssignPT, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPtId == null))
            .ForMember(dest => dest.PTAssignmentPrice, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.PtPrice))
                .ForMember(dest => dest.PtList, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.GymCoursePTs))
            .ForMember(dest => dest.GymCourseId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId))
            ;

        CreateProjection<CustomerPurchased, CustomerPurchasedFreelancePtResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Name))
            .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.ImageUrl))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.FreelancePTPackageId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackageId))
            ;
    }
}
