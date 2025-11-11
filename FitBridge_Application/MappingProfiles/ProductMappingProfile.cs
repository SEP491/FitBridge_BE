using System;
using AutoMapper;
using FitBridge_Application.Features.Products.CreateProduct;
using FitBridge_Application.Dtos.Products;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.MappingProfiles;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductCommand, Product>();

        CreateMap<Product, ProductResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ForMember(dest => dest.IsDisplayed, opt => opt.MapFrom(src => src.IsDisplayed))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
        .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
        .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory.Name))
        .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name));

        CreateMap<Product, ProductByIdResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ForMember(dest => dest.IsDisplayed, opt => opt.MapFrom(src => src.IsDisplayed))
        .ForMember(dest => dest.TotalSold, opt => opt.MapFrom(src => src.ProductDetails.Sum(x => x.SoldQuantity)))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
        .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src => src.CoverImageUrl))
        .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory.Name))
        .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
        .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.Brand.Id))
        .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.SubCategory.Id))
        .ForMember(dest => dest.ProductDetails, opt => opt.MapFrom(src => src.ProductDetails));
    }
}
