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
using FitBridge_Application.Specifications.Bookings.GetGymSlotForBooking;
using FitBridge_Application.Features.Bookings.GetGymSlotForBooking;
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtSchedule;
using FitBridge_Application.Features.Bookings.GetFreelancePtSchedule;
using FitBridge_Application.Features.Bookings.CreateRequestBooking;

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

    /// <summary>
    /// Get all schedule of a customer both freelance pt and gym pt
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    [HttpGet("get-customer-bookings")]
    public async Task<IActionResult> GetCustomerBookings([FromQuery] GetCustomerBookingsParams parameters)
    {
        var result = await _mediator.Send(new GetCustomerBookingsQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetCustomerBookingsResponse>>(StatusCodes.Status200OK.ToString(), "Bookings retrieved successfully", pagination));
    }

    /// <summary>
    /// Get all available gym slots of a gym pt so that customer can know which slots is available to book
    /// </summary>
    /// <param name="parameters">The parameters for the query</param>
    /// <returns>The gym slot for booking</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/bookings/get-gym-slot-for-booking
    ///     {
    ///         "ptId": "01999fdb-fa69-7d1a-ba09-6e186ef7d00b",
    ///         "date": "2025-10-02"
    ///     }
    /// </remarks>
    [HttpGet("get-gym-slot-for-booking")]
    public async Task<IActionResult> GetGymSlotForBooking([FromQuery] GetGymSlotForBookingParams parameters)
    {
        var result = await _mediator.Send(new GetGymSlotForBookingQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetPtGymSlotForBookingResponse>>(StatusCodes.Status200OK.ToString(), "Slot retrieved successfully", pagination));
    }

    [HttpPost("request-booking")]
    [Authorize(Roles = ProjectConstant.UserRoles.Customer + "," + ProjectConstant.UserRoles.FreelancePT)]
    public async Task<IActionResult> CreateRequestBooking([FromBody] CreateRequestBookingCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<List<CreateRequestBookingResponseDto>>(StatusCodes.Status200OK.ToString(), "Booking created successfully", result));
    }

    [HttpGet("freelance-pt-schedule")]
    public async Task<IActionResult> GetFreelancePtSchedule([FromQuery] GetFreelancePtScheduleParams parameters)
    {
        var result = await _mediator.Send(new GetFreelancePtScheduleQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetFreelancePtScheduleResponse>>(StatusCodes.Status200OK.ToString(), "Schedule retrieved successfully", pagination));
    }
}
