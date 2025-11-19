using System;
using AutoMapper;
using FitBridge_Application.Dtos.OrderItems;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.MappingProfiles;

public class OrderItemMappingProfile : Profile
{
    public OrderItemMappingProfile()
    {
        CreateMap<OrderItemDto, OrderItem>();
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<OrderItem, OrderItemForProductOrderResponseDto>();
    }
}
