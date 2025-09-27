using System;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Features.GymSlots.CreateGymSlot;
using FitBridge_API.Helpers.RequestHelpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitBridge_Application.Features.GymSlots.DeleteGymSlotById;
using FitBridge_Application.Features.GymSlots.UpdateGymSlot;
using FitBridge_Application.Specifications.GymSlots;
using FitBridge_Application.Features.GymSlots.GetAllGymSlot;

namespace FitBridge_API.Controllers;

public class GymSlotsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> CreateGymSlot([FromBody] CreateNewSlotResponse request)
    {
        var result = await _mediator.Send(new CreateGymSlotCommand { Request = request });
        return Ok(new BaseResponse<CreateNewSlotResponse>(StatusCodes.Status200OK.ToString(), "Gym slot created successfully", result));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> DeleteGymSlot(string id)
    {
        var result = await _mediator.Send(new DeleteGymSlotByIdCommand(id));
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Gym slot deleted successfully", result));
    }

    [HttpPut]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> UpdateGymSlot([FromBody] UpdateGymSlotCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<CreateNewSlotResponse>(StatusCodes.Status200OK.ToString(), "Gym slot updated successfully", result));
    }

    [HttpGet]
    [Authorize(Roles = "GymOwner, Admin")]
    public async Task<IActionResult> GetGymSlots([FromQuery] GetGymSlotParams parameters)
    {
        var result = await _mediator.Send(new GetGymSlotsQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<CreateNewSlotResponse>>(StatusCodes.Status200OK.ToString(), "Gym slots retrieved successfully", pagination));
    }
}
