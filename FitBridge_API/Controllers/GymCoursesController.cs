using System;
using MediatR;
using FitBridge_Application.Features.GymCourses.Commands;
using Microsoft.AspNetCore.Mvc;
using FitBridge_Application.Dtos.GymCourses.Response;
using FitBridge_API.Helpers.RequestHelpers;
using Microsoft.AspNetCore.Authorization;

namespace FitBridge_API.Controllers;

public class GymCoursesController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "GymOwner")]
    public async Task<IActionResult> CreateGymCourse([FromBody] CreateGymCourseCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new BaseResponse<CreateGymCourseResponse>(StatusCodes.Status200OK.ToString(), "Gym course created successfully", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new BaseResponse<string>(StatusCodes.Status400BadRequest.ToString(), ex.Message, null));
        }
    }
}
