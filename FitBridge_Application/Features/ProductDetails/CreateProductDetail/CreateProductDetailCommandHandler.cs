using System;
using AutoMapper;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.ProductDetails.CreateProductDetail;

public class CreateProductDetailCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUploadService _uploadService, SystemConfigurationService systemConfigurationService) : IRequestHandler<CreateProductDetailCommand, string>
{
    public async Task<string> Handle(CreateProductDetailCommand request, CancellationToken cancellationToken)
    {
        var autoHideProductBeforeExpirationDate = (int)await systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.AutoHideProductBeforeExpirationDate);
        if (request.SalePrice > request.DisplayPrice)
        {
            throw new BusinessException("Sale price cannot be greater than display price");
        }
        if (request.ExpirationDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber <= autoHideProductBeforeExpirationDate)
        {
            throw new BusinessException($"Expiration date is too close to the current date, please set the expiration date to at least {autoHideProductBeforeExpirationDate} days from the current date");
        }
        var imageUrl = request.Image != null ? await _uploadService.UploadFileAsync(request.Image) : null;
        var productDetail = _mapper.Map<CreateProductDetailCommand, ProductDetail>(request);
        productDetail.ImageUrl = imageUrl;
        _unitOfWork.Repository<ProductDetail>().Insert(productDetail);
        await _unitOfWork.CommitAsync();
        return productDetail.Id.ToString();
    }
}
