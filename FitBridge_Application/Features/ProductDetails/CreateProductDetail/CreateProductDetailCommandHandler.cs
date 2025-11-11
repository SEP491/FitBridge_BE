using System;
using AutoMapper;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using MediatR;

namespace FitBridge_Application.Features.ProductDetails.CreateProductDetail;

public class CreateProductDetailCommandHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<CreateProductDetailCommand, string>
{
    public async Task<string> Handle(CreateProductDetailCommand request, CancellationToken cancellationToken)
    {
        var productDetail = _mapper.Map<CreateProductDetailCommand, ProductDetail>(request);
        _unitOfWork.Repository<ProductDetail>().Insert(productDetail);
        await _unitOfWork.CommitAsync();
        return productDetail.Id.ToString();
    }
}
