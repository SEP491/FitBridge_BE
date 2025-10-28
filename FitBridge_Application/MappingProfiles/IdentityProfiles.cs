using System;
using AutoMapper;
using FitBridge_Application.Features.Identities.Registers.RegisterGymPT;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Application.Dtos.Identities;

namespace FitBridge_Application.MappingProfiles;

public class IdentityProfiles : Profile
{
    public IdentityProfiles()
    {
        CreateMap<RegisterGymPtCommand, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CreateNewPT.FullName))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.CreateNewPT.Dob.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => src.CreateNewPT.IsMale))
            .ForMember(dest => dest.GoalTrainings, opt => opt.MapFrom(src => src.CreateNewPT.GoalTrainings.Select(gt => new GoalTraining { Name = gt }).ToList()))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.GymOwnerId, opt => opt.MapFrom(src => Guid.Parse(src.GymOwnerId ?? Guid.Empty.ToString())))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude ?? null))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude ?? null));

        CreateMap<RegisterGymPtCommand, CreateNewPTResponse>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CreateNewPT.FullName))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.CreateNewPT.Dob))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.CreateNewPT.Weight))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.CreateNewPT.Height))
            .ForMember(dest => dest.GoalTrainings, opt => opt.MapFrom(src => src.CreateNewPT.GoalTrainings))
            .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.CreateNewPT.Experience))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.CreateNewPT.IsMale ? "Male" : "Female"));
    }
}
