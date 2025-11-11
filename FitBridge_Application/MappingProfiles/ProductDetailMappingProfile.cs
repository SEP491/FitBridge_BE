using System;
using AutoMapper;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Application.Features.ProductDetails.CreateProductDetail;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.MappingProfiles;

public class ProductDetailMappingProfile : Profile
{
    public ProductDetailMappingProfile()
    {
        CreateMap<ProductDetail, ProductDetailForAdminResponseDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice))
        .ForMember(dest => dest.DisplayPrice, opt => opt.MapFrom(src => src.DisplayPrice))
        .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
        .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
        .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
        .ForMember(dest => dest.WeightId, opt => opt.MapFrom(src => src.WeightId))
        .ForMember(dest => dest.FlavourId, opt => opt.MapFrom(src => src.FlavourId))
        .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
        .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => src.SoldQuantity))
        .ForMember(dest => dest.IsDisplayed, opt => opt.MapFrom(src => src.IsDisplayed))
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
        .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
        .ForMember(dest => dest.WeightUnit, opt => opt.MapFrom(src => src.Weight.Unit))
        .ForMember(dest => dest.WeightValue, opt => opt.MapFrom(src => src.Weight.Value))
        .ForMember(dest => dest.FlavourName, opt => opt.MapFrom(src => src.Flavour.Name));
        
        CreateMap<CreateProductDetailCommand, ProductDetail>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
        .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice))
        .ForMember(dest => dest.DisplayPrice, opt => opt.MapFrom(src => src.DisplayPrice))
        .ForMember(dest => dest.ExpirationDate, opt => opt.MapFrom(src => src.ExpirationDate))
        .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls))
        .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
        .ForMember(dest => dest.WeightId, opt => opt.MapFrom(src => src.WeightId))
        .ForMember(dest => dest.FlavourId, opt => opt.MapFrom(src => src.FlavourId))
        .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
        .ForMember(dest => dest.IsDisplayed, opt => opt.MapFrom(src => src.IsDisplayed));
    }
}
