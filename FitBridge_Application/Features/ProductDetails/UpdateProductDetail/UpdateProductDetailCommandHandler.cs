using System;
using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.ProductDetails.UpdateProductDetail;

public class UpdateProductDetailCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUploadService _uploadService) : IRequestHandler<UpdateProductDetailCommand, string>
{
    public async Task<string> Handle(UpdateProductDetailCommand request, CancellationToken cancellationToken)
    {
        var productDetail = await _unitOfWork.Repository<ProductDetail>().GetByIdAsync(request.Id.Value);
        if (productDetail == null)
        {
            throw new BusinessException("Product detail not found");
        }
        productDetail.OriginalPrice = (request.OriginalPrice != null && request.OriginalPrice.Value > 0) ? request.OriginalPrice.Value : productDetail.OriginalPrice;
        productDetail.DisplayPrice = (request.DisplayPrice != null && request.DisplayPrice.Value > 0) ? request.DisplayPrice.Value : productDetail.DisplayPrice;
        productDetail.ExpirationDate = request.ExpirationDate ?? productDetail.ExpirationDate;
        if(request.ImageUrl != null)
        {
            productDetail.ImageUrl = await _uploadService.UploadFileAsync(request.ImageUrl);
        }
        if(request.SalePrice != null && request.SalePrice.Value > request.DisplayPrice.Value)
        {
            throw new BusinessException("Sale price cannot be greater than display price");
        }
        productDetail.SalePrice = (request.SalePrice != null && request.SalePrice.Value > 0) ? request.SalePrice.Value : productDetail.SalePrice;
        productDetail.ProductId = request.ProductId ?? productDetail.ProductId;
        productDetail.WeightId = request.WeightId ?? productDetail.WeightId;
        productDetail.FlavourId = request.FlavourId ?? productDetail.FlavourId;
        productDetail.Quantity = request.Quantity ?? productDetail.Quantity;
        productDetail.IsDisplayed = request.IsDisplayed ?? productDetail.IsDisplayed;
        productDetail.ServingSizeInformation = request.ServingSizeInformation ?? productDetail.ServingSizeInformation;
        productDetail.ServingsPerContainerInformation = request.ServingsPerContainerInformation ?? productDetail.ServingsPerContainerInformation;
        productDetail.ProteinPerServingGrams = request.ProteinPerServingGrams ?? productDetail.ProteinPerServingGrams;
        productDetail.CaloriesPerServingKcal = request.CaloriesPerServingKcal ?? productDetail.CaloriesPerServingKcal;
        productDetail.BCAAPerServingGrams = request.BCAAPerServingGrams ?? productDetail.BCAAPerServingGrams;
        productDetail.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<ProductDetail>().Update(productDetail);
        await _unitOfWork.CommitAsync();
        return "Product detail updated successfully";
    }
}
