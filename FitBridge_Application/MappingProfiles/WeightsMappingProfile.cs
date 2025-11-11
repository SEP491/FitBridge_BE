using System;
using AutoMapper;
using FitBridge_Application.Dtos.Weights;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.MappingProfiles;

public class WeightsMappingProfile : Profile
{
    public WeightsMappingProfile()
    {
        CreateProjection<Weight, WeightResponseDto>();
    }
}
