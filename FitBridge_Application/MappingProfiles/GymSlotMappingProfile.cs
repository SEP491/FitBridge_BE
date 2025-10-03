using System;
using AutoMapper;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles;

public class GymSlotMappingProfile : Profile
{
    public GymSlotMappingProfile()
    {
        CreateMap<CreateNewSlotResponse, GymSlot>();
        CreateMap<GymSlot, CreateNewSlotResponse>();
        CreateMap<GymSlot, SlotResponseDto>();
        CreateMap<SlotResponseDto, GymSlot>();
        CreateMap<GymSlot, GetPTSlot>()
        .ForMember(dest => dest.SlotId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GymOwner.FullName))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.IsActivated, opt => opt.MapFrom(src => src.PTGymSlots.Any()));
    }
}
