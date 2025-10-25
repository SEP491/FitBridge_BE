using System;
using AutoMapper;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Dtos.CustomerPurchaseds;
using FitBridge_Domain.Enums.GymCourses;
using FitBridge_Application.Dtos.FreelancePTPackages;

namespace FitBridge_Application.MappingProfiles;

public class CustomerPurchasedMappingProfile : Profile
{
    public CustomerPurchasedMappingProfile()
    {
        CreateProjection<CustomerPurchased, GymCoursesPtResponse>()
            .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId ?? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackageId))
            .ForMember(dest => dest.GymOwnerId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.GymOwnerId : Guid.Empty))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Name : src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Price : src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Price))
            .ForMember(dest => dest.GymPtId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPtId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Description : src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.ImageUrl : src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.ImageUrl))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.Pt, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPtId != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPt : src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Pt))
            .ForMember(dest => dest.PackageType, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId != null ? PackageType.GymCourse : PackageType.FreelancePTPackage));

        CreateProjection<CustomerPurchased, CustomerPurchasedResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Name))
            .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.ImageUrl))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.CanAssignPT, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPtId == null))
            .ForMember(dest => dest.PTAssignmentPrice, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.PtPrice))
            .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPtId))
            .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPt.FullName))
            .ForMember(dest => dest.PtImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymPt.AvatarUrl))
            .ForMember(dest => dest.GymCourseId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourseId))
            ;

        CreateProjection<CustomerPurchased, CustomerPurchasedFreelancePtResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Name))
            .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.ImageUrl))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.FreelancePTPackageId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackageId))
            .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.PtId : Guid.Empty))
            .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Pt.FullName : string.Empty))
            .ForMember(dest => dest.PtImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage != null ? src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Pt.AvatarUrl : string.Empty));

        CreateProjection<CustomerPurchased, GetCustomerPurchasedForFreelancePt>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Name))
            .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.ImageUrl))
            .ForMember(dest => dest.AvailableSessions, opt => opt.MapFrom(src => src.AvailableSessions))
            .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
            .ForMember(dest => dest.FreelancePTPackageId, opt => opt.MapFrom(src => src.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackageId))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.CustomerImageUrl, opt => opt.MapFrom(src => src.Customer.AvatarUrl));
    }
}
