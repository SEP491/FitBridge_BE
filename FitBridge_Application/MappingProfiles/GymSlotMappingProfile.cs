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

        CreateMap<PTGymSlot, GymSlotPtBookingDto>()
        .ForMember(dest => dest.PtGymSlot, opt => opt.MapFrom(src => src))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.FullName
        : string.Empty))
        .ForMember(dest => dest.CustomerAvatarUrl, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.AvatarUrl
        : string.Empty))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.Id
        : Guid.Empty))
        .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => src.Booking.SessionStatus));
        
        CreateMap<PTGymSlot, GymPtRegisterSlot>()
        .ForMember(dest => dest.PtGymSlotId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.GymSlotId, opt => opt.MapFrom(src => src.GymSlotId))
        .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => src.RegisterDate))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.GymSlot.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.GymSlot.EndTime))
        .ForMember(dest => dest.SlotName, opt => opt.MapFrom(src => src.GymSlot.Name));
    }
}
