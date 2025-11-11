using System;
using FitBridge_Application.Dtos.Categories;
using MediatR;

namespace FitBridge_Application.Features.Categories.GetAllSubCat;

public class GetAllSubCategoriesQuery : IRequest<List<SubCategoryResponseDto>>
{
}
