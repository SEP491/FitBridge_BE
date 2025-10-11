using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Features.ActivitySets.CreateActivitySet;
using FitBridge_Application.Features.ActivitySets.UpdateActivityProgress;
using FitBridge_Application.Features.ActivitySets.UpdateActivitySet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class ActivitySetsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateActivitySet([FromBody] CreateActivitySetCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<ActivitySetResponseDto>(StatusCodes.Status200OK.ToString(), "Activity set created successfully", result));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateActivitySet([FromBody] UpdateActivitySetCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<ActivitySetResponseDto>(StatusCodes.Status200OK.ToString(), "Activity set updated successfully", result));
    }

    [HttpPut("activity-progress")]
    public async Task<IActionResult> UpdateActivityProgress([FromBody] UpdateActivityProgressCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<List<ActivitySetResponseDto>>(StatusCodes.Status200OK.ToString(), "Activity progress updated successfully", result));
    }
}
