using System;
using AutoMapper;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.MappingProfiles;

public class GymSlotMappingProfile : Profile
{
    public GymSlotMappingProfile()
    {
        CreateMap<CreateNewSlotResponse, GymSlot>();
        CreateMap<GymSlot, CreateNewSlotResponse>();
    }
}
