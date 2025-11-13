using System;
using FitBridge_Application.Dtos.ProductDetails;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductDetailForSale;

public class GetProductDetailForSaleQuery : IRequest<ProductDetailForSaleResponseDto>
{
    public Guid ProductId { get; set; }
}
