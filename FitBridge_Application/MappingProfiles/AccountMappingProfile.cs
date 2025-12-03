using System;
using AutoMapper;
using FitBridge_Application.Dtos.Accounts;
using FitBridge_Application.Dtos.Accounts.Customers;
using FitBridge_Application.Dtos.Accounts.FreelancePts;
using FitBridge_Application.Dtos.Accounts.HotResearch;
using FitBridge_Application.Dtos.Accounts.Profiles;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Dtos.GymPTs;
using FitBridge_Application.Features.Accounts.UpdateProfiles;
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
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Bio : null))
            .ForMember(dest => dest.PriceFrom, opt => opt.MapFrom(src => src.PTFreelancePackages.Count > 0 ? src.PTFreelancePackages.Min(x => x.Price) : 0))
            .ForMember(dest => dest.ExperienceYears, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Experience : 0))
            .ForMember(dest => dest.Certifications, opt => opt.MapFrom(src => src.PtCertificates))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Reviews.Count > 0 ? src.Reviews.Average(x => x.Rating) : 0))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude));

        CreateProjection<ApplicationUser, GetAllGymPtsResponseDto>()
            .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Experience : 0))
            .ForMember(dest => dest.GoalTrainings, opt => opt.MapFrom(src => src.GoalTrainings.Select(x => x.Name).ToList()))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.GymReviews.Count > 0 ? src.GymReviews.Average(x => x.Rating) : 0))
            .ForMember(dest => dest.TotalCoursesAssigned, opt => opt.MapFrom(src => src.GymCoursePTs.Count))
            .ForMember(dest => dest.GymName, opt => opt.MapFrom(src => src.GymOwner != null ? src.GymOwner.GymName : null))
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Bio : null));

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

        CreateMap<ApplicationUser, GetCustomersDto>();

        CreateMap<UpdateProfileCommand, ApplicationUser>();

        CreateMap<ApplicationUser, UpdateProfileResponseDto>()
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Bio : null));

        CreateProjection<ApplicationUser, GetAllGymPtsForAdminResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => src.IsMale))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Experience : 0))
            .ForMember(dest => dest.GymOwnerId, opt => opt.MapFrom(src => src.GymOwnerId != null ? src.GymOwnerId : null))
            .ForMember(dest => dest.GymOwnerName, opt => opt.MapFrom(src => src.GymOwner != null ? src.GymOwner.GymName : null))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<ApplicationUser, GetGymPtsDetailForAdminResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => src.IsMale))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.GymOwnerId, opt => opt.MapFrom(src => src.GymOwnerId != null ? src.GymOwnerId : null))
            .ForMember(dest => dest.GoalTrainings, opt => opt.MapFrom(src => src.GoalTrainings.Select(x => x.Name).ToList()))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.GymOwnerName, opt => opt.MapFrom(src => src.GymOwner != null ? src.GymOwner.GymName : null));

        CreateProjection<ApplicationUser, GetAllGymOwnerForAdminDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.GymName, opt => opt.MapFrom(src => src.GymName))
            .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

        CreateMap<ApplicationUser, GetGymOwnerDetailForAdminDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.GymName, opt => opt.MapFrom(src => src.GymName))
            .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.hotResearch, opt => opt.MapFrom(src => src.hotResearch))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.GymDescription, opt => opt.MapFrom(src => src.GymDescription))
            .ForMember(dest => dest.MinimumSlot, opt => opt.MapFrom(src => src.MinimumSlot))
            .ForMember(dest => dest.GymImages, opt => opt.MapFrom(src => src.GymImages));

        CreateProjection<ApplicationUser, GetAllCustomersForAdminDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => src.IsMale))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<ApplicationUser, GetUserProfileResponse>()
            .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Bio : null))
            .ForMember(dest => dest.FrontCitizenIdUrl, opt => opt.MapFrom(src => src.FrontCitizenIdUrl))
            .ForMember(dest => dest.BackCitizenIdUrl, opt => opt.MapFrom(src => src.BackCitizenIdUrl))
            .ForMember(dest => dest.CitizenIdNumber, opt => opt.MapFrom(src => src.CitizenIdNumber))
            .ForMember(dest => dest.IdentityCardPlace, opt => opt.MapFrom(src => src.IdentityCardPlace))
            .ForMember(dest => dest.CitizenCardPermanentAddress, opt => opt.MapFrom(src => src.CitizenCardPermanentAddress))
            .ForMember(dest => dest.IdentityCardDate, opt => opt.MapFrom(src => src.IdentityCardDate))
            .ForMember(dest => dest.BusinessAddress, opt => opt.MapFrom(src => src.BusinessAddress))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.IsMale ? "Male" : "Female"))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.GymFoundationDate, opt => opt.MapFrom(src => src.GymFoundationDate))
            .ForMember(dest => dest.TaxCode, opt => opt.MapFrom(src => src.TaxCode))
            .ForMember(dest => dest.GymDescription, opt => opt.MapFrom(src => src.GymDescription))
            .ForMember(dest => dest.GymName, opt => opt.MapFrom(src => src.GymName))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Weight : 0))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.UserDetail != null ? src.UserDetail.Height : 0));
    }
}