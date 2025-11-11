using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Specifications.Products.GetProductForSales;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductForSale;

public class GetProductForSaleQuery(GetProductForSaleParams parameters) : IRequest<PagingResultDto<ProductForSaleResponseDto>>  
{
    public GetProductForSaleParams Parameters { get; set; } = parameters;
}
