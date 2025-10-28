using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.UserGoals;
using FitBridge_Application.Features.UserGoals;
using FitBridge_Application.Features.UserGoals.CheckUserGoals;
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

/// <summary>
/// Update user goal
/// </summary>
/// <param name="command"></param>
/// <param name="customerPurchasedId">Customer purchased id</param>
/// <returns></returns>
    [HttpPut("{customerPurchasedId}")]
    public async Task<IActionResult> UpdateUserGoal([FromBody] UpdateUserGoalCommand command, [FromRoute] Guid customerPurchasedId)
    {
        command.CustomerPurchasedId = customerPurchasedId;
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<UserGoalsDto>(StatusCodes.Status200OK.ToString(), "User goal updated successfully", result));
    }

    [HttpGet("{customerPurchasedId}")]
    public async Task<IActionResult> GetUserGoalById([FromRoute] Guid customerPurchasedId)
    {
        var result = await _mediator.Send(new GetUserGoalByIdQuery { CustomerPurchasedId = customerPurchasedId });
        return Ok(new BaseResponse<UserGoalsDto>(StatusCodes.Status200OK.ToString(), "User goal retrieved successfully", result));
    }

    [HttpGet("check/{customerPurchasedId}")]
    public async Task<IActionResult> CheckUserGoal([FromRoute] Guid customerPurchasedId)
    {
        var result = await _mediator.Send(new CheckUserGoalQuery { CustomerPurchasedId = customerPurchasedId });
        if(result)
        {
            return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "User goal found", result));
        }
        else
        {
            return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "User goal not found", result));
        }
    }
}
