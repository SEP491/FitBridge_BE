using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Categories;
using FitBridge_Application.Features.Categories.GetAllCategory;
using FitBridge_Application.Features.Categories.GetAllSubCat;
using FitBridge_Application.Features.Categories.CreateCategory;
using FitBridge_Application.Features.Categories.UpdateCategory;
using FitBridge_Application.Features.Categories.DeleteCategory;
using FitBridge_Application.Features.Categories.CreateSubCategory;
using FitBridge_Application.Features.Categories.UpdateSubCategory;
using FitBridge_Application.Features.Categories.DeleteSubCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class CategoriesController(IMediator _mediator) : _BaseApiController
{
    #region Category Endpoints

    /// <summary>
    /// Get all categories
    /// </summary>
    /// <returns>List of all categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<List<CategoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(new BaseResponse<List<CategoryResponseDto>>(StatusCodes.Status200OK.ToString(), "Categories retrieved successfully", result));
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="command">The category details</param>
    /// <returns>The created category ID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/categories
    ///     {
    ///         "name": "Supplements"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Category created successfully</response>
    /// <response code="400">Category with the same name already exists</response>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResponse<CreateCategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateCategoryResponseDto>(StatusCodes.Status200OK.ToString(), "Category created successfully", result));
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <param name="command">The updated category details</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/v1/categories/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "name": "Nutritional Supplements"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Category updated successfully</response>
    /// <response code="400">Category with the same name already exists</response>
    /// <response code="404">Category not found</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Category updated successfully", result));
    }

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    /// <param name="id">The category ID</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/categories/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// This performs a soft delete, setting IsEnabled to false.
    /// The category will no longer appear in the active categories list but remains in the database.
    /// </remarks>
    /// <response code="200">Category deleted successfully</response>
    /// <response code="404">Category not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand { Id = id });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Category deleted successfully", result));
    }

    #endregion

    #region SubCategory Endpoints

    /// <summary>
    /// Get all subcategories
    /// </summary>
    /// <returns>List of all subcategories</returns>
    [HttpGet("sub-categories")]
    [ProducesResponseType(typeof(BaseResponse<List<SubCategoryResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSubCategories()
    {
        var result = await _mediator.Send(new GetAllSubCategoriesQuery());
        return Ok(new BaseResponse<List<SubCategoryResponseDto>>(StatusCodes.Status200OK.ToString(), "Sub categories retrieved successfully", result));
    }

    /// <summary>
    /// Create a new subcategory
    /// </summary>
    /// <param name="command">The subcategory details</param>
    /// <returns>The created subcategory ID</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/categories/sub-categories
    ///     {
    ///         "name": "Protein Powder",
    ///         "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">SubCategory created successfully</response>
    /// <response code="400">SubCategory with the same name already exists in this category</response>
    /// <response code="404">Category not found</response>
    [HttpPost("sub-categories")]
    [ProducesResponseType(typeof(BaseResponse<CreateSubCategoryResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateSubCategoryResponseDto>(StatusCodes.Status200OK.ToString(), "SubCategory created successfully", result));
    }

    /// <summary>
    /// Update an existing subcategory
    /// </summary>
    /// <param name="id">The subcategory ID</param>
    /// <param name="command">The updated subcategory details</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/v1/categories/sub-categories/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     {
    ///         "name": "Whey Protein",
    ///         "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">SubCategory updated successfully</response>
    /// <response code="400">SubCategory with the same name already exists in this category</response>
    /// <response code="404">SubCategory or Category not found</response>
    [HttpPut("sub-categories/{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubCategory([FromRoute] Guid id, [FromBody] UpdateSubCategoryCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "SubCategory updated successfully", result));
    }

    /// <summary>
    /// Delete a subcategory (soft delete)
    /// </summary>
    /// <param name="id">The subcategory ID</param>
    /// <returns>Success status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/v1/categories/sub-categories/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///
    /// This performs a soft delete, setting IsEnabled to false.
    /// The subcategory will no longer appear in the active subcategories list but remains in the database.
    /// </remarks>
    /// <response code="200">SubCategory deleted successfully</response>
    /// <response code="404">SubCategory not found</response>
    [HttpDelete("sub-categories/{id}")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSubCategory([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteSubCategoryCommand { Id = id });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "SubCategory deleted successfully", result));
    }

    #endregion
}
