using System;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Dtos.SessionActivities;
using FitBridge_Application.Features.SessionActivities;
using FitBridge_Application.Features.SessionActivities.GetSessionActivityById;
using FitBridge_Application.Features.SessionActivities.UpdateSessionActivity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers;

public class SessionActivitiesController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Creates a new session activity for a training booking
    /// </summary>
    /// <param name="command">The session activity creation request containing:
    /// - BookingId: The ID of the booking this activity belongs to
    /// - ActivityType: The type of activity (WarmUp, Workout, or WithEquiment)
    /// - ActivityName: The name/description of the activity
    /// - MuscleGroups: List of target muscle groups (Biceps, ForeArm, Thigh, Calf, Chest, Waist, Hip, Shoulder, Legs)
    /// - ActivitySets: Collection of sets with number of reps and weight lifted</param>
    /// <returns>Returns a SessionActivityResponseDto containing the created activity details including:
    /// - Id: The unique identifier of the created session activity
    /// - ActivityType: The type of activity performed
    /// - ActivityName: The name of the activity
    /// - MuscleGroups: List of targeted muscle groups as strings
    /// - BookingId: The associated booking ID
    /// - ActivitySets: Collection of activity sets with their details</returns>
    [HttpPost]
    [Authorize(Roles = ProjectConstant.UserRoles.FreelancePT)]
    public async Task<IActionResult> CreateSessionActivity([FromBody] CreateSessionActivityCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<SessionActivityResponseDto>(StatusCodes.Status200OK.ToString(), "Session activity created successfully", result));
    }

    [HttpPut]
    [Authorize(Roles = ProjectConstant.UserRoles.FreelancePT)]
    public async Task<IActionResult> UpdateSessionActivity([FromBody] UpdateSessionActivityCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<SessionActivityResponseDto>(StatusCodes.Status200OK.ToString(), "Session activity updated successfully", result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSessionActivityById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetSessionActivityByIdQuery { Id = id });
        return Ok(new BaseResponse<SessionActivityResponseDto>(StatusCodes.Status200OK.ToString(), "Session activity retrieved successfully", result));
    }
}
