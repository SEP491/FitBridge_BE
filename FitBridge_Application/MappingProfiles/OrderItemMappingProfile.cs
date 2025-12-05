using System;
using AutoMapper;
using FitBridge_Application.Dtos.Dashboards;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItemDto, OrderItem>();
        CreateMap<OrderItem, OrderItemDto>()
        .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
        .ForMember(dest => dest.OriginalProductPrice, opt => opt.MapFrom(src => src.OriginalProductPrice))
        .ForMember(dest => dest.ProductDetailId, opt => opt.MapFrom(src => src.ProductDetailId))
        .ForMember(dest => dest.GymCourseId, opt => opt.MapFrom(src => src.GymCourseId))
        .ForMember(dest => dest.SubscriptionPlansInformationId, opt => opt.MapFrom(src => src.SubscriptionPlansInformationId))
        .ForMember(dest => dest.FreelancePTPackageId, opt => opt.MapFrom(src => src.FreelancePTPackageId))
        .ForMember(dest => dest.GymPtId, opt => opt.MapFrom(src => src.GymPtId));
        CreateMap<OrderItem, OrderItemForProductOrderResponseDto>()
        .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductDetail.Product.Name));

        CreateMap<OrderItem, OrderItemForCourseOrderResponseDto>()
        .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.GymCourse.Name))
        .ForMember(dest => dest.FreelancePTPackage, opt => opt.MapFrom(src => src.FreelancePTPackage));
    }
}