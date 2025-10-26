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
        CreateMap<UpdateUserDetailDto, UserDetail>()
        .ForMember(dest => dest.Biceps, opt => opt.Condition(src => src.Biceps != null))
        .ForMember(dest => dest.ForeArm, opt => opt.Condition(src => src.ForeArm != null))
        .ForMember(dest => dest.Thigh, opt => opt.Condition(src => src.Thigh != null))
        .ForMember(dest => dest.Calf, opt => opt.Condition(src => src.Calf != null))
        .ForMember(dest => dest.Chest, opt => opt.Condition(src => src.Chest != null))
        .ForMember(dest => dest.Waist, opt => opt.Condition(src => src.Waist != null))
        .ForMember(dest => dest.Hip, opt => opt.Condition(src => src.Hip != null))
        .ForMember(dest => dest.Shoulder, opt => opt.Condition(src => src.Shoulder != null))
        .ForMember(dest => dest.Height, opt => opt.Condition(src => src.Height != null))
        .ForMember(dest => dest.Weight, opt => opt.Condition(src => src.Weight != null))
        .ForMember(dest => dest.Certificates, opt => opt.Condition(src => src.Certificates != null))
        .ForMember(dest => dest.Experience, opt => opt.Condition(src => src.Experience != null));
        CreateMap<UserDetail, UpdateUserDetailDto>();
    }
}
