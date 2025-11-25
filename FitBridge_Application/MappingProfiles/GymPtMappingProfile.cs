using System;
using AutoMapper;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Application.Dtos.GymPTs;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Dtos.GymSlots;

namespace FitBridge_Application.MappingProfiles;

public class GymPtMappingProfile : Profile
{
    public GymPtMappingProfile()
    {
        CreateProjection<ApplicationUser, GymPtResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.IsMale, opt => opt.MapFrom(src => src.IsMale))
            .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
            .ForMember(dest => dest.Experience, opt => opt.MapFrom(src => src.UserDetail!.Experience));
        CreateProjection<PTGymSlot, GymPtRegisterSlot>()
            .ForMember(dest => dest.PtGymSlotId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.GymSlotId, opt => opt.MapFrom(src => src.GymSlotId))
            .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => src.RegisterDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.GymSlot.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.GymSlot.EndTime))
            .ForMember(dest => dest.SlotName, opt => opt.MapFrom(src => src.GymSlot.Name));
    }
}
