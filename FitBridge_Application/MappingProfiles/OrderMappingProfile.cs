using System;
using AutoMapper;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Features.Orders.CreateOrders;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Application.Dtos.OrderItems;

namespace FitBridge_Application.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<CreateOrderCommand, Order>();
        CreateMap<CreatePaymentRequestDto, Order>();
        CreateMap<Order, OrderResponseDto>();
        CreateMap<Order, GetAllProductOrderResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.CheckoutUrl, opt => opt.MapFrom(src => src.CheckoutUrl))
        .ForMember(dest => dest.SubTotalPrice, opt => opt.MapFrom(src => src.SubTotalPrice))
        .ForMember(dest => dest.ShippingFee, opt => opt.MapFrom(src => src.ShippingFee))
        .ForMember(dest => dest.ShippingFeeActualCost, opt => opt.MapFrom(src => src.ShippingFeeActualCost))
        .ForMember(dest => dest.ShippingTrackingId, opt => opt.MapFrom(src => src.ShippingTrackingId))
        .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
        .ForMember(dest => dest.CurrentStatus, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.CouponId, opt => opt.MapFrom(src => src.CouponId))
        .ForMember(dest => dest.ShippingDetail, opt => opt.MapFrom(src => src.Address));
        CreateMap<Order, CourseOrderResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.CheckoutUrl, opt => opt.MapFrom(src => src.CheckoutUrl))
        .ForMember(dest => dest.SubTotalPrice, opt => opt.MapFrom(src => src.SubTotalPrice))
        .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
        .ForMember(dest => dest.CouponId, opt => opt.MapFrom(src => src.CouponId))
        .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.Coupon != null ? src.Coupon.DiscountPercent : 0))
        .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
    }
}
