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
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.PTGymSlotId, opt => opt.MapFrom(src => src.PTGymSlotId))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => src.SessionStatus))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.StartTime : src.PtFreelanceStartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.EndTime : src.PtFreelanceEndTime))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.FullName : (string?)null))
        .ForMember(dest => dest.PtAvatarUrl, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.AvatarUrl : (string?)null));

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
        .ForMember(dest => dest.CustomerAvatarUrl, opt => opt.MapFrom(src => src.Customer.AvatarUrl))
        .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.CustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Name));

        CreateProjection<Booking, GetGymPtScheduleResponse>()
        .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.BookingName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.StartTime : (TimeOnly?)null))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.EndTime : (TimeOnly?)null))
        .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
        .ForMember(dest => dest.CustomerPurchasedId, opt => opt.MapFrom(src => src.CustomerPurchasedId))
        .ForMember(dest => dest.SessionStatus, opt => opt.MapFrom(src => src.SessionStatus))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : null))
        .ForMember(dest => dest.CustomerAvatarUrl, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.AvatarUrl : null))
        .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.CustomerPurchased != null && src.CustomerPurchased.OrderItems.Any() ? src.CustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).First().GymCourse.Name : null))
        .ForMember(dest => dest.PTGymSlotId, opt => opt.MapFrom(src => src.PTGymSlotId))
        .ForMember(dest => dest.GymSlotName, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.Name : null))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip));

        CreateProjection<Booking, GetBookingHistoryResponseDto>()
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.StartTime : src.PtFreelanceStartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.EndTime : src.PtFreelanceEndTime))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : null))
        .ForMember(dest => dest.CustomerAvatarUrl, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.AvatarUrl : null))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.Pt != null ? src.Pt.FullName : (src.PTGymSlot != null && src.PTGymSlot.PT != null ? src.PTGymSlot.PT.FullName : null)))
        .ForMember(dest => dest.PtAvatarUrl, opt => opt.MapFrom(src => src.Pt != null ? src.Pt.AvatarUrl : (src.PTGymSlot != null && src.PTGymSlot.PT != null ? src.PTGymSlot.PT.AvatarUrl : null)))
        .ForMember(dest => dest.GymSlotName, opt => opt.MapFrom(src => src.PTGymSlot != null && src.PTGymSlot.GymSlot != null ? src.PTGymSlot.GymSlot.Name : null))
        .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.CustomerPurchased != null && src.CustomerPurchased.OrderItems.Any() ? src.CustomerPurchased.OrderItems.OrderByDescending(x => x.CreatedAt).First().FreelancePTPackage.Name : null));

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
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.FullName : null))
        .ForMember(dest => dest.PtAvatarUrl, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.AvatarUrl : null))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerId != null ? src.Customer.FullName : null))
        .ForMember(dest => dest.CustomerAvatarUrl, opt => opt.MapFrom(src => src.CustomerId != null ? src.Customer.AvatarUrl : null))
        .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
        .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.RequestType, opt => opt.MapFrom(src => src.RequestType))
        .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.RequestStatus))
        .ForMember(dest => dest.OriginalBooking, opt => opt.MapFrom(src => src.TargetBooking != null ? src.TargetBooking : null))
        .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate));

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
        .ForMember(dest => dest.ActualStartTime, opt => opt.MapFrom(src => src.SessionStartTime))
        .ForMember(dest => dest.ActualEndTime, opt => opt.MapFrom(src => src.SessionEndTime))
        .ForMember(dest => dest.SetsPlan, opt => opt.MapFrom(src => src.SessionActivities.Count))
        .ForMember(dest => dest.SetsCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Count(y => y.IsCompleted))))
        .ForMember(dest => dest.RestTime, opt => opt.MapFrom(src => src.SessionActivities.Sum(x => x.ActivitySets.Sum(y => y.RestTime ?? 0))))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip))
        .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.PtName, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.FullName : null))
        .ForMember(dest => dest.PtAvatarUrl, opt => opt.MapFrom(src => src.PtId != null ? src.Pt.AvatarUrl : null))
        .ForPath(dest => dest.RepsProgress.RepsCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.NumOfReps ?? 0 : 0))))
        .ForPath(dest => dest.WeightLiftedProgress.WeightLiftedCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? (a.WeightLifted ?? 0) * (a.NumOfReps ?? 0) : 0))))
        .ForPath(dest => dest.RepsProgress.RepsPlan, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.PlannedNumOfReps ?? 0))))
        .ForPath(dest => dest.WeightLiftedProgress.WeightLiftedPlan, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => (a.WeightLifted ?? 0) * (a.PlannedNumOfReps ?? 0)))))
        .ForPath(dest => dest.PracticeTimeProgress.PracticeTimeCompleted, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.PracticeTime ?? 0 : 0))))
        .ForPath(dest => dest.PracticeTimeProgress.PracticeTimePlan, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.PlannedPracticeTime ?? 0))));

        CreateMap<Booking, SessionReportDto>()
        .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.SessionName, opt => opt.MapFrom(src => src.BookingName))
        .ForMember(dest => dest.DateTraining, opt => opt.MapFrom(src => src.BookingDate))
        .ForMember(dest => dest.PlannedStartTime, opt => opt.MapFrom(src => src.PtFreelanceStartTime))
        .ForMember(dest => dest.PlannedEndTime, opt => opt.MapFrom(src => src.PtFreelanceEndTime))
        .ForMember(dest => dest.ActualStartTime, opt => opt.MapFrom(src => src.SessionStartTime))
        .ForMember(dest => dest.ActualEndTime, opt => opt.MapFrom(src => src.SessionEndTime))
        .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
        .ForMember(dest => dest.NutritionTip, opt => opt.MapFrom(src => src.NutritionTip))
        .ForPath(dest => dest.SessionTotalSummary.SessionActivityCount, opt => opt.MapFrom(src => src.SessionActivities.Count))
        .ForPath(dest => dest.SessionTotalSummary.TotalCompletedSets, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Count(a => a.IsCompleted))))
        .ForPath(dest => dest.SessionTotalSummary.TotalCompletedReps, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.NumOfReps ?? 0 : 0))))
        .ForPath(dest => dest.SessionTotalSummary.TotalPlannedPracticeTimeSec, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.PlannedPracticeTime ?? 0))))
        .ForPath(dest => dest.SessionTotalSummary.TotalCompletedPracticeTimeSec, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.PracticeTime ?? 0 : 0))))
        .ForPath(dest => dest.SessionTotalSummary.TotalRestTimeSec, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.RestTime ?? 0 : 0))))
        .ForPath(dest => dest.SessionTotalSummary.PlannedSets, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Count)))
        .ForPath(dest => dest.SessionTotalSummary.PlannedReps, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.PlannedNumOfReps ?? 0))))
        .ForPath(dest => dest.SessionTotalSummary.TotalPlannedDistanceMeters, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.PlannedDistance ?? 0))))
        .ForPath(dest => dest.SessionTotalSummary.TotalCompletedDistanceMeters, opt => opt.MapFrom(src => src.SessionActivities.Sum(s => s.ActivitySets.Sum(a => a.IsCompleted ? a.ActualDistance ?? 0 : 0))))
        .ForMember(dest => dest.ActivityTypesPerformed, opt => opt.MapFrom(src => src.SessionActivities.Select(s => s.ActivityType).Distinct().ToList()))
        .ForPath(dest => dest.ActivitiesSummary, opt => opt.MapFrom(src => src.SessionActivities.Select(s => new ActivitySummaryDto
        {
            SessionActivityId = s.Id,
            ActivityName = s.ActivityName,
            ActivityType = s.ActivityType,
            MuscleGroup = s.MuscleGroup,
            CompletedSets = s.ActivitySets.Count(a => a.IsCompleted),
            CompletedReps = s.ActivitySets.Sum(a => a.IsCompleted ? a.NumOfReps ?? 0 : 0),
            PlannedSets = s.ActivitySets.Count,
            PlannedReps = s.ActivitySets.Sum(a => a.PlannedNumOfReps ?? 0),
            HeaviestWeightLifted = s.ActivitySets.Max(a => a.IsCompleted ? a.WeightLifted ?? 0 : 0),
            LightestWeightLifted = s.ActivitySets.Min(a => a.IsCompleted ? a.WeightLifted ?? 0 : 0),
            LongestDistanceMeters = s.ActivitySets.Max(a => a.IsCompleted ? a.ActualDistance ?? 0 : 0),
            ShortestDistanceMeters = s.ActivitySets.Min(a => a.IsCompleted ? a.ActualDistance ?? 0 : 0),
            LongestPracticeTimeSeconds = s.ActivitySets.Max(a => a.IsCompleted ? a.PracticeTime ?? 0 : 0),
            ShortestPracticeTimeSeconds = s.ActivitySets.Min(a => a.IsCompleted ? a.PracticeTime ?? 0 : 0),
            PlannedDistanceMeters = s.ActivitySets.Sum(a => a.PlannedDistance ?? 0),
            PlannedPracticeTimeSeconds = s.ActivitySets.Sum(a => a.PlannedPracticeTime ?? 0),
            CompletedDistanceMeters = s.ActivitySets.Sum(a => a.IsCompleted ? a.ActualDistance ?? 0 : 0),
            CompletedPracticeTimeSeconds = s.ActivitySets.Sum(a => a.IsCompleted ? a.PracticeTime ?? 0 : 0),

        }).ToList()));
    }
}
