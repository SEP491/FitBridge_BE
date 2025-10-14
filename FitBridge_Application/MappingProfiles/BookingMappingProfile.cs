using System;
using AutoMapper;
using FitBridge_Application.Dtos.Bookings;
using FitBridge_Application.Features.Bookings.RequestEditBooking;
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

        CreateMap<BookingRequest, EditBookingResponseDto>()
        .ForMember(dest => dest.BookingRequestId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.TargetBookingId, opt => opt.MapFrom(src => src.TargetBookingId))
        .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
        .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.RequestStatus));


        CreateMap<BookingRequest, UpdateBookingResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
        .ForMember(dest => dest.TargetBookingId, opt => opt.MapFrom(src => src.TargetBookingId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

        CreateProjection<BookingRequest, GetBookingRequestResponse>()
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
        .ForMember(dest => dest.TargetBookingId, opt => opt.MapFrom(src => src.TargetBookingId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
        .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.RequestStatus))
        .ForMember(dest => dest.OriginalBooking, opt => opt.MapFrom(src => src.TargetBooking != null ? src.TargetBooking : null));

        CreateProjection<Booking, BookingResponseDto>()
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.PtFreelanceStartTime, opt => opt.MapFrom(src => src.PtFreelanceStartTime))
        .ForMember(dest => dest.PtFreelanceEndTime, opt => opt.MapFrom(src => src.PtFreelanceEndTime))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

        CreateMap<Booking, UpdateBookingResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.PtId, opt => opt.MapFrom(src => src.PtId))
        .ForMember(dest => dest.TargetBookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PtFreelanceStartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PtFreelanceEndTime))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note));

        CreateMap<Booking, TrainingResultResponseDto>()
        .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PtFreelanceStartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PtFreelanceEndTime))
        .ForMember(dest => dest.SetsPlan, opt => opt.MapFrom(src => src.SessionActivities.Count))
        .ForMember(dest => dest.SetsCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Count(y => y.IsCompleted))))
        .ForMember(dest => dest.PracticeTime, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Sum(y => y.PracticeTime ?? 0))))
        .ForMember(dest => dest.RestTime, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Sum(y => y.RestTime ?? 0))))
        .ForMember(dest => dest.WeightLifted, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Sum(y => y.WeightLifted ?? 0))))
        .ForMember(dest => dest.NumOfReps, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Sum(y => y.NumOfReps ?? 0))))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip))
        .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.FullName : null))
        .ForMember(dest => dest.PtAvatarUrl, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.AvatarUrl : null))
        .ForMember(dest => dest.RepsProgress.RepsCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.NumOfReps ?? 0 : 0))))   
        .ForMember(dest => dest.WeightLiftedProgress.WeightLiftedCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.WeightLifted ?? 0 : 0))))
        .ForMember(dest => dest.RepsProgress.RepsPlan, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.NumOfReps ?? 0))))
        .ForMember(dest => dest.WeightLiftedProgress.WeightLiftedPlan, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.WeightLifted ?? 0))));
    }
}
