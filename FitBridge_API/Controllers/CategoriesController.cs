using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Categories;
using FitBridge_Application.Features.Categories.GetAllCategory;
using FitBridge_Application.Features.Categories.GetAllSubCat;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class CategoriesController(IMediator _mediator) : _BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(new BaseResponse<List<CategoryResponseDto>>(StatusCodes.Status200OK.ToString(), "Categories retrieved successfully", result));
    }

    [HttpGet("sub-categories")]
    public async Task<IActionResult> GetAllSubCategories()
    {
        var result = await _mediator.Send(new GetAllSubCategoriesQuery());
        return Ok(new BaseResponse<List<SubCategoryResponseDto>>(StatusCodes.Status200OK.ToString(), "Sub categories retrieved successfully", result));
    }
}
