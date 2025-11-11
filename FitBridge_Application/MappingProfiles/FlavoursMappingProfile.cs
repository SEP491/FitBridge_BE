using System;
using AutoMapper;
using FitBridge_Application.Dtos.Flavours;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.MappingProfiles;

public class FlavoursMappingProfile : Profile   
{
    public FlavoursMappingProfile()
    {
        CreateProjection<Flavour, FlavourResponseDto>();
    }
}
