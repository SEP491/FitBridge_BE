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
        CreateMap<UserGoal, UserGoalsDto>();
        CreateMap<CreateUserGoalCommand, UserGoal>();
    }
}
