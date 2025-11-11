using System;
using FitBridge_Application.Dtos;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Specifications.Products.GetAllProductForAdmin;
using MediatR;

namespace FitBridge_Application.Features.Products.GetAllProductForAdmin;

public class GetAllProductForAdminQuery(GetAllProductForAdminQueryParams queryParams) : IRequest<PagingResultDto<ProductResponseDto>>
{
    public GetAllProductForAdminQueryParams QueryParams { get; set; } = queryParams;
}
