using System;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.ProductDetails.GetProductDetailById;

public class GetProductDetailByIdQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper, SystemConfigurationService systemConfigurationService) : IRequestHandler<GetProductDetailByIdQuery, ProductDetailForAdminResponseDto>
{
    public async Task<ProductDetailForAdminResponseDto> Handle(GetProductDetailByIdQuery request, CancellationToken cancellationToken)
    {
        var productDetail = await _unitOfWork.Repository<ProductDetail>().GetByIdAsync(request.Id, false, new List<string> { "Product", "Weight", "Flavour" }) ?? throw new NotFoundException(nameof(ProductDetail), request.Id);
        int nearExpiredDateProductWarning = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.NearExpiredDateProductWarning);
        var productDetailDto = _mapper.Map<ProductDetail, ProductDetailForAdminResponseDto>(productDetail);

        productDetailDto.DaysToExpire = productDetail.ExpirationDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
        productDetailDto.IsNearExpired = productDetailDto.DaysToExpire <= nearExpiredDateProductWarning;

        return productDetailDto;
    }
}
