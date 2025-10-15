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
        CreateMap<ActivitySetRequestDto, ActivitySet>()
        .ForMember(dest => dest.PlannedNumOfReps, opt => opt.MapFrom(src => src.PlannedNumOfReps ?? 0))
        .ForMember(dest => dest.PlannedPracticeTime, opt => opt.MapFrom(src => src.PlannedPracticeTime ?? 0));
        CreateMap<ActivitySet, ActivitySetResponseDto>()
        .ForMember(dest => dest.PlannedNumOfReps, opt => opt.MapFrom(src => src.PlannedNumOfReps ?? 0))
        .ForMember(dest => dest.PlannedPracticeTime, opt => opt.MapFrom(src => src.PlannedPracticeTime ?? 0))
        .ForMember(dest => dest.NumOfReps, opt => opt.MapFrom(src => src.NumOfReps ?? 0))
        .ForMember(dest => dest.PracticeTime, opt => opt.MapFrom(src => src.PracticeTime ?? 0));
        CreateMap<ActivitySetResponseDto, ActivitySet>();
        CreateMap<CreateActivitySetCommand, ActivitySet>();
    }
}
