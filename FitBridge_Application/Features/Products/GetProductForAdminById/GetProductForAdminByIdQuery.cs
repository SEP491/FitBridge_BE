using System;
using FitBridge_Application.Dtos.Products;
using MediatR;

namespace FitBridge_Application.Features.Products.GetProductForAdminById;

public class GetProductForAdminByIdQuery : IRequest<ProductByIdResponseDto>
{
    public Guid Id { get; set; }
}
