using System;
using FitBridge_Application.Dtos.Categories;
using MediatR;

namespace FitBridge_Application.Features.Categories.GetAllCategory;

public class GetAllCategoriesQuery : IRequest<List<CategoryResponseDto>>
{
}
