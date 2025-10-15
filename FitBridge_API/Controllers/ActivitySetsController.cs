using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.ActivitySets;
using FitBridge_Application.Features.ActivitySets.CreateActivitySet;
using FitBridge_Application.Features.ActivitySets.DeleteActivity;
using FitBridge_Application.Features.ActivitySets.GetActivitySetById;
using FitBridge_Application.Features.ActivitySets.GetActivitySetBySessionId;
using FitBridge_Application.Features.ActivitySets.UpdateActivityProgress;
using FitBridge_Application.Features.ActivitySets.UpdateActivitySet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class ActivitySetsController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Use to add Activity Set in session detail screen, use when pt freelance want to edit session activity and add more activity sets
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateActivitySet([FromBody] CreateActivitySetCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<ActivitySetResponseDto>(StatusCodes.Status200OK.ToString(), "Activity set created successfully", result));
    }

    /// <summary>
    /// Update activity set in session detail screen where pt create Session activity and activity set
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateActivitySet([FromBody] UpdateActivitySetCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<ActivitySetResponseDto>(StatusCodes.Status200OK.ToString(), "Activity set updated successfully", result));
    }

    /// <summary>
    /// Update activity set when customer practice, pass is completed = true so that activity set is marked as completed
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("activity-progress")]
    public async Task<IActionResult> UpdateActivityProgress([FromBody] UpdateActivityProgressCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<ActivitySetResponseDto>(StatusCodes.Status200OK.ToString(), "Activity progress updated successfully", result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetActivitySetById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetActivitySetByIdQuery { Id = id });
        return Ok(new BaseResponse<ActivitySetResponseDto>(StatusCodes.Status200OK.ToString(), "Activity set retrieved successfully", result));
    }

    /// <summary>
    /// Get all activity sets by session activity id
    /// </summary>
    /// <param name="sessionActivityId"></param>
    /// <returns></returns>
    [HttpGet("session-activity/{sessionActivityId}")]
    public async Task<IActionResult> GetActivitySetsBySessionActivityId([FromRoute] Guid sessionActivityId)
    {
        var result = await _mediator.Send(new GetActivitySetsBySessionActivityIdQuery { SessionActivityId = sessionActivityId });
        return Ok(new BaseResponse<List<ActivitySetResponseDto>>(StatusCodes.Status200OK.ToString(), "Activity sets retrieved successfully", result));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivitySet([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new DeleteActivitySetByIdCommand { Id = id });
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Activity set deleted successfully", result));
    }
}
