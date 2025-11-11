using System;
using FitBridge_Application.Dtos.Brands;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Specifications.Brands.GetAllBrands;

namespace FitBridge_Application.Features.Brands.GetAllBrands;

public class GetAllBrandsQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllBrandsQuery, List<BrandResponseDto>>
{
    public async Task<List<BrandResponseDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var brands = await _unitOfWork.Repository<Brand>().GetAllWithSpecificationProjectedAsync<BrandResponseDto>(new GetAllBrandsSpecification(), _mapper.ConfigurationProvider);
        return brands.ToList();
    }
}
