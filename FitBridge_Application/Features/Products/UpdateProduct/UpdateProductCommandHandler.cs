using System;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Dtos.Products;
using FitBridge_Domain.Exceptions;
using MediatR;
using AutoMapper;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Products.UpdateProduct;

public class UpdateProductCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper, IUploadService _uploadService) : IRequestHandler<UpdateProductCommand, ProductResponseDto>
{
    public async Task<ProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id.Value);
        if (product == null)
        {
            throw new BusinessException("Product not found");
        }
        if (request.CoverImage != null)
        {
            var coverImageUrl = await _uploadService.UploadFileAsync(request.CoverImage);
            product.CoverImageUrl = coverImageUrl;
        }
        product.Name = request.Name ?? product.Name;
        product.Description = request.Description ?? product.Description;
        product.BrandId = request.BrandId ?? product.BrandId;
        product.ProteinSources = request.ProteinSources ?? product.ProteinSources;
        product.CountryOfOrigin = request.CountryOfOrigin ?? product.CountryOfOrigin;
        product.SubCategoryId = request.SubCategoryId ?? product.SubCategoryId;
        product.IsDisplayed = request.IsDisplayed ?? product.IsDisplayed;
        product.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Product>().Update(product);
        await _unitOfWork.CommitAsync();
        return _mapper.Map<ProductResponseDto>(product);
    }
}
