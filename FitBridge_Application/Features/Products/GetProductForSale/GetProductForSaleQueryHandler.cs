using System;
using AutoMapper;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Products.GetProductForSales;
using FitBridge_Domain.Entities.Ecommerce;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductForSale;

public class GetProductForSaleQueryHandler(IUnitOfWork unitOfWork, IMapper _mapper) : IRequestHandler<GetProductForSaleQuery, PagingResultDto<ProductForSaleResponseDto>>    
{
    public async Task<PagingResultDto<ProductForSaleResponseDto>> Handle(GetProductForSaleQuery request, CancellationToken cancellationToken)
    {
        var spec = new GetProductForSaleSpec(request.Parameters);
        var products = await unitOfWork.Repository<Product>().GetAllWithSpecificationAsync(spec);
        var productDtos = new List<ProductForSaleResponseDto>();
        foreach (var product in products)
        {
            var productDto = _mapper.Map<ProductForSaleResponseDto>(product);
            if (product.ProductDetails.Count == 0)
            {
                continue;
            }
            var cheapestProductDetail = product.ProductDetails.OrderBy(pd => pd.SalePrice).FirstOrDefault();
            productDto.SalePrice = cheapestProductDetail.SalePrice;
            productDto.Quantity = cheapestProductDetail.Quantity;
            productDto.DisplayPrice = cheapestProductDetail.DisplayPrice;
            productDtos.Add(productDto);
        }
        var totalItems = await unitOfWork.Repository<Product>().CountAsync(spec);
        return new PagingResultDto<ProductForSaleResponseDto>(totalItems, productDtos);
    }
}
