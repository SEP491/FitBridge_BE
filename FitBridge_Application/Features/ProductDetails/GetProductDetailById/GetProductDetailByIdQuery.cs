using System;
using FitBridge_Application.Dtos.ProductDetails;
using MediatR;

namespace FitBridge_Application.Features.ProductDetails.GetProductDetailById;

public class GetProductDetailByIdQuery : IRequest<ProductDetailForAdminResponseDto>
{
    public Guid Id { get; set; }
}
