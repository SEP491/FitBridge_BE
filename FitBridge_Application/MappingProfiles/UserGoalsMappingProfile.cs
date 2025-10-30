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
        .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
        .ForMember(dest => dest.FinalImageUrl, opt => opt.MapFrom(src => src.FinalImageUrl));
        
        CreateMap<CreateUserGoalCommand, UserGoal>();
    }
}
