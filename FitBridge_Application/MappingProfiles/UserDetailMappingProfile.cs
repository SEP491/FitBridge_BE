using System;
using AutoMapper;
using FitBridge_Application.Dtos.Accounts.UserDetails;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.MappingProfiles;

public class UserDetailMappingProfile : Profile
{
    public UserDetailMappingProfile()
    {
        CreateMap<UserDetail, UserDetailDto>();
    }
}
