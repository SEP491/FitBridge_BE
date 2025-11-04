using System;
using AutoMapper;
using FitBridge_Application.Dtos.Subscriptions;
using FitBridge_Domain.Entities.ServicePackages;

namespace FitBridge_Application.MappingProfiles;

public class SubscriptionsMappingProfile : Profile
{
    public SubscriptionsMappingProfile()
    {
        CreateMap<SubscriptionPlansInformation, SubscriptionPlanResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.PlanName))
        .ForMember(dest => dest.PlanCharge, opt => opt.MapFrom(src => src.PlanCharge))
        .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
        .ForMember(dest => dest.LimitUsage, opt => opt.MapFrom(src => src.LimitUsage))
        .ForMember(dest => dest.FeatureKeyId, opt => opt.MapFrom(src => src.FeatureKeyId))
        .ForMember(dest => dest.FeatureKeyName, opt => opt.MapFrom(src => src.FeatureKey.FeatureName));

        CreateMap<UserSubscription, CurrentUserSubscriptionResponseDto>()
        .ForMember(dest => dest.UserSubscriptionId, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
        .ForMember(dest => dest.SubscriptionPlanId, opt => opt.MapFrom(src => src.SubscriptionPlanId))
        .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
        .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
        .ForMember(dest => dest.UserSubscriptionLimitUsage, opt => opt.MapFrom(src => src.LimitUsage))
        .ForMember(dest => dest.UserSubscriptionCurrentUsage, opt => opt.MapFrom(src => src.CurrentUsage))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
        
        CreateProjection<UserSubscription, UserSubscriptionHistoryResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.PlanName))
        .ForMember(dest => dest.PlanCharge, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.PlanCharge))
        .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.Duration))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.Description))
        .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.ImageUrl))
        .ForMember(dest => dest.UserSubscriptionLimitUsage, opt => opt.MapFrom(src => src.LimitUsage))
        .ForMember(dest => dest.FeatureKeyId, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.FeatureKeyId))
        .ForMember(dest => dest.FeatureKeyName, opt => opt.MapFrom(src => src.SubscriptionPlansInformation.FeatureKey.FeatureName))
        .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
        .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
        .ForMember(dest => dest.CurrentUsage, opt => opt.MapFrom(src => src.CurrentUsage))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));
    }
}
