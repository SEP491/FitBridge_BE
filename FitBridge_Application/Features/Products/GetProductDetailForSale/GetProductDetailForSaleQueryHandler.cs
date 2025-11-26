using System;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Application.Dtos.Reviews;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Products.GetProductDetailForSale;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductDetailForSale;

public class GetProductDetailForSaleQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, SystemConfigurationService systemConfigurationService) : IRequestHandler<GetProductDetailForSaleQuery, ProductDetailForSaleResponseDto>
{
    public async Task<ProductDetailForSaleResponseDto> Handle(GetProductDetailForSaleQuery request, CancellationToken cancellationToken)
    {
        int nearExpiredDateProductWarning = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.NearExpiredDateProductWarning);
        var product = await _unitOfWork.Repository<Product>().GetBySpecificationAsync(new GetProductDetailForSaleSpec(request.ProductId));
        if (product == null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }
        var productDetailForSaleDto = _mapper.Map<ProductDetailForSaleResponseDto>(product);
        var productDetailDtos = new List<ProductDetailForAdminResponseDto>();
        foreach (var productDetail in product.ProductDetails)
        {
            var productDetailDto = _mapper.Map<ProductDetailForAdminResponseDto>(productDetail);
            productDetailDto.DaysToExpire = productDetail.ExpirationDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
            productDetailDto.IsNearExpired = productDetailDto.DaysToExpire <= nearExpiredDateProductWarning;
            productDetailDtos.Add(productDetailDto);
        }
        productDetailForSaleDto.ProductDetails = productDetailDtos;
        return productDetailForSaleDto;
    }
}
