using System;
using FitBridge_Application.Dtos.Categories;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Application.Specifications.Categories.GetAllCategories;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Application.Features.Categories.GetAllCategory;

public class GetAllCategoriesQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponseDto>>
{
    public async Task<List<CategoryResponseDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Repository<Category>().GetAllWithSpecificationProjectedAsync<CategoryResponseDto>(new GetAllCategoriesSpecification(), _mapper.ConfigurationProvider);
        return categories.ToList();
    }
}
