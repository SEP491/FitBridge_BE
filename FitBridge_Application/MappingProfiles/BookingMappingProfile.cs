using System;
using AutoMapper;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.MappingProfiles;

public class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, GetCustomerBookingsResponse>()
        .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.SlotName, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.Name : (string?)null))
        .ForMember(dest => dest.PtFreelanceStartTime, opt => opt.MapFrom(src => src.PtFreelanceStartTime))
        .ForMember(dest => dest.PtFreelanceEndTime, opt => opt.MapFrom(src => src.PtFreelanceEndTime))
        .ForMember(dest => dest.PTGymSlotId, opt => opt.MapFrom(src => src.PTGymSlotId))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => src.SessionStatus))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip))
        .ForMember(dest => dest.GymSlotStartTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.StartTime : (TimeOnly?)null))
        .ForMember(dest => dest.GymSlotEndTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.EndTime : (TimeOnly?)null))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.PT != null ? src.PTGymSlot.PT.FullName : (string?)null))
        .ForMember(dest => dest.PtAvatarUrl, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.PT != null ? src.PTGymSlot.PT.AvatarUrl : (string?)null));

        CreateMap<Booking, GetFreelancePtScheduleResponse>()
        .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.PtFreelanceStartTime, opt => opt.MapFrom(src => src.PtFreelanceStartTime))
        .ForMember(dest => dest.PtFreelanceEndTime, opt => opt.MapFrom(src => src.PtFreelanceEndTime))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => src.SessionStatus))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
        .ForMember(dest => dest.CustomerAvatarUrl, opt => opt.MapFrom(src => src.Customer.AvatarUrl));

        CreateMap<BookingRequest, CreateRequestBookingResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType));

        CreateMap<BookingRequest, Booking>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.PtFreelanceStartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.PtFreelanceEndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId));
    }
}
