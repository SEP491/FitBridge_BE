using System;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Products.GetAllProductForAdmin;
using FitBridge_Domain.Entities.Ecommerce;
using MediatR;

namespace FitBridge_Application.Features.Products.GetAllProductForAdmin;

public class GetAllProductForAdminQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<GetAllProductForAdminQuery, PagingResultDto<ProductResponseDto>>
{
    public async Task<PagingResultDto<ProductResponseDto>> Handle(GetAllProductForAdminQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetAllProductForAdminSpec(request.QueryParams);
        var products = await unitOfWork.Repository<Product>().GetAllWithSpecificationAsync(spec);
        var productDtos = _mapper.Map<List<ProductResponseDto>>(products);
        var totalItems = await unitOfWork.Repository<Product>().CountAsync(spec);
        return new PagingResultDto<ProductResponseDto>(totalItems, productDtos);
    }
}
