using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Application.Features.ProductDetails.CreateProductDetail;
using FitBridge_Application.Features.ProductDetails.GetProductDetailById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class ProductDetailsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProductDetail([FromForm] CreateProductDetailCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Product detail created successfully", result));
    }

    [HttpGet("admin/{id}")]
    public async Task<IActionResult> GetProductDetailById([FromRoute] Guid id)
    {
        var query = new GetProductDetailByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(new BaseResponse<ProductDetailForAdminResponseDto>(StatusCodes.Status200OK.ToString(), "Product detail retrieved successfully", result));
    }
}
