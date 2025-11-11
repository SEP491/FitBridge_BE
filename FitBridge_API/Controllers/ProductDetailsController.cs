using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Application.Features.ProductDetails.CreateProductDetail;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class ProductDetailsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProductDetail([FromBody] CreateProductDetailCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<string>(StatusCodes.Status200OK.ToString(), "Product detail created successfully", result));
    }
}
