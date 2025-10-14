using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Features.UserGoals;
using FitBridge_Application.Features.UserGoals.GetUserGoalById;
using FitBridge_Application.Features.UserGoals.UpdateUserGoals;
using FitBridge_Application.Specifications.UserGoals;
using FitBridge_Domain.Entities.Trainings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class UserGoalsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateUserGoal([FromBody] CreateUserGoalCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<UserGoalsDto>(StatusCodes.Status200OK.ToString(), "User goal created successfully", result));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserGoal([FromBody] UpdateUserGoalCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<UserGoalsDto>(StatusCodes.Status200OK.ToString(), "User goal updated successfully", result));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserGoalById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetUserGoalByIdQuery { Id = id });
        return Ok(new BaseResponse<UserGoalsDto>(StatusCodes.Status200OK.ToString(), "User goal retrieved successfully", result));
    }
}
