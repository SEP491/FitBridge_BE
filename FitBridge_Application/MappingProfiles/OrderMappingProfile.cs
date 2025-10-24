using System;
using AutoMapper;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Features.Orders.CreateOrders;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<CreateOrderCommand, Order>();
        CreateMap<CreatePaymentRequestDto, Order>();
        CreateMap<Order, OrderResponseDto>();
    }
}
