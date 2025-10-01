using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Features.Bookings.CancelGymPtBooking;
using FitBridge_Application.Specifications.Bookings.GetCustomerBookings;
using FitBridge_Application.Features.Bookings.GetCustomerBooking;
using FitBridge_Application.Dtos.Bookings;

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

    [HttpGet("get-customer-bookings")]
    public async Task<IActionResult> GetCustomerBookings([FromQuery] GetCustomerBookingsParams parameters)
    {
        var result = await _mediator.Send(new GetCustomerBookingsQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetCustomerBookingsResponse>>(StatusCodes.Status200OK.ToString(), "Bookings retrieved successfully", pagination));
    }
}
