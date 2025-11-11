using System;

using FitBridge_API.Controllers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Products;
using FitBridge_Application.Features.Products.CreateProduct;
using FitBridge_Application.Features.Products.GetAllProductForAdmin;
using FitBridge_Application.Features.Products.GetProductForAdminById;
using FitBridge_Application.Specifications.Products.GetAllProductForAdmin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class ProductsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Product created successfully", product));
    }

    [HttpGet("admin")]
    public async Task<IActionResult> GetAllProductForAdmin([FromQuery] GetAllProductForAdminQueryParams queryParams)
    {
        var query = new GetAllProductForAdminQuery(queryParams);
        var products = await _mediator.Send(query);
        var pagination = ResultWithPagination(products.Items, products.Total, queryParams.Page, queryParams.Size);
        return Ok(new BaseResponse<Pagination<ProductResponseDto>>(StatusCodes.Status200OK.ToString(), "Products retrieved successfully", pagination));
    }
    
    [HttpGet("admin/{id}")]
    public async Task<IActionResult> GetProductForAdminById([FromRoute] Guid id)
    {
        var query = new GetProductForAdminByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(new BaseResponse<ProductByIdResponseDto>(StatusCodes.Status200OK.ToString(), "Product retrieved successfully", result));
    }
}
