using System;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Interfaces.Repositories;

namespace FitBridge_Application.Features.Products.CreateProduct;

public class CreateProductCommandHandler(IMapper _mapper, IUnitOfWork _unitOfWork) : IRequestHandler<CreateProductCommand, string>
{
    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Product>(request);
        _unitOfWork.Repository<Product>().Insert(product);
        await _unitOfWork.CommitAsync();
        return product.Id.ToString();
    }

}
