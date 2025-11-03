using System;
using FitBridge_Application.Dtos.SystemConfigs;
using FitBridge_Domain.Entities.Systems;
using AutoMapper;

namespace FitBridge_Application.MappingProfiles;

public class SystemConfigurationMappingProfile : Profile
{
    public SystemConfigurationMappingProfile()
    {
        CreateMap<SystemConfiguration, SystemConfigurationDto>();
    }
}
