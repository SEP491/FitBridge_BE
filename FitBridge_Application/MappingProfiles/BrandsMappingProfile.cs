using System;
using AutoMapper;
using FitBridge_Application.Dtos.Brands;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.MappingProfiles;

public class BrandsMappingProfile : Profile
{
    public BrandsMappingProfile()
    {
        CreateProjection<Brand, BrandResponseDto>();
    }
}
