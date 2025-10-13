using System;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Features.SessionActivities;
using FitBridge_Domain.Entities.Trainings;
using AutoMapper;

namespace FitBridge_Application.MappingProfiles;

public class SessionActivityMappingProfile : Profile
{
    public SessionActivityMappingProfile()
    {
        CreateMap<CreateSessionActivityCommand, SessionActivity>();
        CreateMap<SessionActivity, SessionActivityResponseDto>();
    }
}
