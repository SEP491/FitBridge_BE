using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Features.Bookings.CancelGymPtBooking;

namespace FitBridge_API.Controllers;

public class BookingsController(IMediator _mediator) : _BaseApiController
{
    [HttpPost("cancel-booking")]
    [Authorize(Roles = ProjectConstant.UserRoles.Customer + "," + ProjectConstant.UserRoles.GymPT)]
    public async Task<IActionResult> CancelBooking([FromBody] CancelGymPtBookingCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Booking cancelled successfully", result));
    }
}
