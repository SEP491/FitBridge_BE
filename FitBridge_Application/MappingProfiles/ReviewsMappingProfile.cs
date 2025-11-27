using System;
using AutoMapper;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.MappingProfiles;

public class ReviewsMappingProfile : Profile    
{
    public ReviewsMappingProfile()
    {
        CreateMap<Review, ReviewProductResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
        .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
        .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.IsEdited))
        .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
        .ForMember(dest => dest.UserAvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
        .ForPath(dest => dest.ProductDetail.FlavourName, opt => opt.MapFrom(src => src.ProductDetail != null ? src.ProductDetail.Flavour.Name : null))
        .ForPath(dest => dest.ProductDetail.WeightUnit, opt => opt.MapFrom(src => src.ProductDetail != null ? src.ProductDetail.Weight.Unit : null))
        .ForPath(dest => dest.ProductDetail.WeightValue, opt => opt.MapFrom(src => src.ProductDetail != null ? (double)src.ProductDetail.Weight.Value : 0));
    }
}
