using System;
using AutoMapper;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductForAdminById;

public class GetProductForAdminByIdQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper): IRequestHandler<GetProductForAdminByIdQuery, ProductByIdResponseDto>
{
    public async Task<ProductByIdResponseDto> Handle(GetProductForAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>().GetByIdAsync(request.Id, false, new List<string> { "Brand", "SubCategory", "ProductDetails", "ProductDetails.Weight", "ProductDetails.Flavour" }) ?? throw new NotFoundException(nameof(Product), request.Id);
        
        return _mapper.Map<ProductByIdResponseDto>(product);
    }

}
