using System;
using AutoMapper;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles;

public class GymSlotPtMappingProfile : Profile
{
    public GymSlotPtMappingProfile()
    {
        CreateMap<PTGymSlot, GetPTSlotResponse>()
        .ForMember(dest => dest.PtGymSlotId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
    src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.FullName
        : string.Empty))
        .ForMember(dest => dest.IsBooking, opt => opt.MapFrom(src => src.Booking != null));

        CreateProjection<PTGymSlot, PTSlotScheduleResponse>()
        .ForMember(dest => dest.PtGymSlotId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.GymSlot.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.GymSlot.EndTime))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.FullName
        : string.Empty))
        .ForMember(dest => dest.CustomerAvatar, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.AvatarUrl
        : string.Empty))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Booking != null && src.Booking.Customer != null
        ? src.Booking.Customer.Id
        : Guid.Empty));
        
        CreateProjection<PTGymSlot, GetPtGymSlotForBookingResponse>()
        .ForMember(dest => dest.PtGymSlotId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.SlotId, opt => opt.MapFrom(src => src.GymSlot.Id))
        .ForMember(dest => dest.PTId, opt => opt.MapFrom(src => src.PTId))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PT.FullName))
        .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.PT.AvatarUrl))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.GymSlot.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.GymSlot.EndTime))
        .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => src.RegisterDate));
    }
}
