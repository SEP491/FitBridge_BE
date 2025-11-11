using System;
using FitBridge_Application.Dtos.Brands;
using MediatR;

namespace FitBridge_Application.Features.Brands.GetAllBrands;

public class GetAllBrandsQuery : IRequest<List<BrandResponseDto>>
{
}
