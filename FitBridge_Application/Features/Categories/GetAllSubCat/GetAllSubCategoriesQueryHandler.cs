using FitBridge_Application.Dtos.Categories;
using FitBridge_Application.Interfaces.Repositories;
using MediatR;
using AutoMapper;
using FitBridge_Domain.Entities.Ecommerce;
using FitBridge_Application.Specifications.Categories.GetAllSubCategories;

namespace FitBridge_Application.Features.Categories.GetAllSubCat;

public class GetAllSubCategoriesQueryHandler(IUnitOfWork _unitOfWork, IMapper _mapper) : IRequestHandler<GetAllSubCategoriesQuery, List<SubCategoryResponseDto>>
{
    public async Task<List<SubCategoryResponseDto>> Handle(GetAllSubCategoriesQuery request, CancellationToken cancellationToken)
    {
        var subCategories = await _unitOfWork.Repository<SubCategory>().GetAllWithSpecificationProjectedAsync<SubCategoryResponseDto>(new GetAllSubCategoriesSpecification(), _mapper.ConfigurationProvider);
        return subCategories.ToList();
    }
}
