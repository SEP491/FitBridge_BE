using System;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Features.ActivitySets.CreateActivitySet;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;

namespace FitBridge_Application.MappingProfiles;

public class ActivitySetMappingProfile : Profile
{
    public ActivitySetMappingProfile()
    {
        CreateMap<ActivitySetRequestDto, ActivitySet>();
        CreateMap<ActivitySet, ActivitySetResponseDto>();
        CreateMap<ActivitySetResponseDto, ActivitySet>();
        CreateMap<CreateActivitySetCommand, ActivitySet>();
    }
}
