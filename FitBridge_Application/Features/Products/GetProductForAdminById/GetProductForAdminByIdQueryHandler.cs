using System;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductForAdminById;

public class GetProductForAdminByIdQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper, SystemConfigurationService systemConfigurationService): IRequestHandler<GetProductForAdminByIdQuery, ProductByIdResponseDto>
{
    public async Task<ProductByIdResponseDto> Handle(GetProductForAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(request.Id, false, new List<string> { "Brand", "SubCategory", "ProductDetails", "ProductDetails.Weight", "ProductDetails.Flavour" }) ?? throw new NotFoundException(nameof(Product), request.Id);
        int nearExpiredDateProductWarning = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.NearExpiredDateProductWarning);
        var productDto = _mapper.Map<ProductByIdResponseDto>(product);

        foreach (var productDetail in productDto.ProductDetails)
        {
            productDetail.DaysToExpire = productDetail.ExpirationDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;
            productDetail.IsNearExpired = productDetail.DaysToExpire <= nearExpiredDateProductWarning;
        }
        
        return productDto;
    }

}
