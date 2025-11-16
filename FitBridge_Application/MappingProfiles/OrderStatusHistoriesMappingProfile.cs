using System;
using AutoMapper;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles;

public class OrderStatusHistoriesMappingProfile : Profile
{
    public OrderStatusHistoriesMappingProfile()
    {
        CreateMap<OrderStatusHistory, OrderStatusResponseDto>();
    }
}
