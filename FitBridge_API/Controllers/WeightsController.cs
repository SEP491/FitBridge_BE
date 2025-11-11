using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Weights;
using FitBridge_Application.Features.Weights.GetAllWeights;
namespace FitBridge_API.Controllers;

public class WeightsController(IMediator _mediator) : _BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllWeights()
    {
        var result = await _mediator.Send(new GetAllWeightsQuery());
        return Ok(new BaseResponse<List<WeightResponseDto>>(StatusCodes.Status200OK.ToString(), "Weights retrieved successfully", result));
    }
}
