using System;
using AutoMapper;
using FitBridge_Application.Dtos.Categories;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.MappingProfiles;

public class CategoriesMappingProfile : Profile
{
    public CategoriesMappingProfile()
    {
        CreateProjection<Category, CategoryResponseDto>();
        CreateProjection<SubCategory, SubCategoryResponseDto>();
    }
}
