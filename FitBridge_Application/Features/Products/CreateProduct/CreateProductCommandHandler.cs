using System;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;

namespace FitBridge_Application.Features.Products.CreateProduct;

public class CreateProductCommandHandler(IMapper _mapper, IUnitOfWork _unitOfWork, IUploadService _uploadService) : IRequestHandler<CreateProductCommand, string>
{
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var coverImageUrl = request.CoverImage != null ? await _uploadService.UploadFileAsync(request.CoverImage) : null;
        var product = _mapper.Map<Product>(request);
        product.CoverImageUrl = coverImageUrl;
        _unitOfWork.Repository<Product>().Insert(product);
        await _unitOfWork.CommitAsync();
        return product.Id.ToString();
    }
}
