using System;
using AutoMapper;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Features.UserGoals;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.MappingProfiles;

public class UserGoalsMappingProfile : Profile
{
    public UserGoalsMappingProfile()
    {
        CreateMap<UserGoal, UserGoalsDto>()
        .ForMember(dest => dest.CurrentBiceps, opt => opt.MapFrom(src => src.FinalBiceps))
        .ForMember(dest => dest.CurrentForeArm, opt => opt.MapFrom(src => src.FinalForeArm))
        .ForMember(dest => dest.CurrentThigh, opt => opt.MapFrom(src => src.FinalThigh))
        .ForMember(dest => dest.CurrentCalf, opt => opt.MapFrom(src => src.FinalCalf))
        .ForMember(dest => dest.CurrentChest, opt => opt.MapFrom(src => src.FinalChest))
        .ForMember(dest => dest.CurrentWaist, opt => opt.MapFrom(src => src.FinalWaist))
        .ForMember(dest => dest.CurrentHip, opt => opt.MapFrom(src => src.FinalHip))
        .ForMember(dest => dest.CurrentShoulder, opt => opt.MapFrom(src => src.FinalShoulder))
        .ForMember(dest => dest.CurrentHeight, opt => opt.MapFrom(src => src.FinalHeight))
        .ForMember(dest => dest.CurrentWeight, opt => opt.MapFrom(src => src.FinalWeight));
        
        CreateMap<CreateUserGoalCommand, UserGoal>();
    }
}
