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
using FitBridge_Application.Features.Bookings.AcceptBookingRequestCommand;
using FitBridge_Application.Features.Bookings.RequestEditBooking;
using FitBridge_Application.Features.Bookings.AcceptEditBookingRequest;
using FitBridge_Application.Specifications.Bookings.GetBookingRequests;
using FitBridge_Application.Features.Bookings.GetBookingRequest;
using FitBridge_Application.Features.Bookings.CreateBooking;

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

    /// <summary>
    /// Create a booking request for a freelance pt or customer can be used by Freelance Pt or Customer
    /// </summary>
    /// <param name="command"></param>
    /// <returns>On success, return the booking request</returns>
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
    /// <summary>
    /// Accept a pending booking request
    /// </summary>
    /// <param name="command">The command containing the booking request ID to accept</param>
    /// <returns>The ID of the accepted booking request</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/bookings/accept-booking-request
    ///     {
    ///         "bookingRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    ///     
    /// Used by Freelance PT or Customer to accept a booking request created by the other party.
    /// When accepted:
    /// - A new confirmed Booking is created
    /// - The BookingRequest status changes to "Approved"
    /// - Available sessions are decremented from CustomerPurchased
    /// 
    /// Validation checks:
    /// - Request must be in "Pending" status
    /// - Request type must be "CustomerCreate" or "PtCreate"
    /// - No time slot conflicts for both customer and PT
    /// - Customer must have available sessions
    /// </remarks>
    /// <response code="200">Booking request accepted successfully</response>
    /// <response code="400">Invalid request (e.g., not pending, wrong type, conflicts)</response>
    /// <response code="404">Booking request not found</response>
    [HttpPost("accept-booking-request")]
    public async Task<IActionResult> AcceptBookingRequest([FromBody] AcceptBookingRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<Guid>(StatusCodes.Status200OK.ToString(), "Booking request accepted successfully", result));
    }

    /// <summary>
    /// Create a request to edit an existing booking
    /// </summary>
    /// <param name="command">The command containing the target booking ID and new booking details</param>
    /// <returns>Details of the created edit request</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/bookings/request-edit-booking
    ///     {
    ///         "targetBookingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "bookingName": "Updated Morning Session",
    ///         "note": "Rescheduling due to personal conflict"
    ///     }
    ///     
    /// Used by Freelance PT or Customer to propose changes to an existing booking.
    /// The other party must accept the edit request for changes to take effect.
    /// Creates a BookingRequest with type "CustomerUpdate" or "PtUpdate".
    /// </remarks>
    [HttpPost("request-edit-booking")]
    public async Task<IActionResult> RequestEditBooking([FromBody] RequestEditBookingCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<EditBookingResponseDto>(StatusCodes.Status200OK.ToString(), "Booking request edited successfully", result));
    }

    /// <summary>
    /// Accept a request to edit an existing booking
    /// </summary>
    /// <param name="command">The command containing the edit request ID to accept</param>
    /// <returns>The updated booking details</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/bookings/accept-edit-booking
    ///     {
    ///         "bookingRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    ///     
    /// Used by Freelance PT or Customer to accept an edit request created by the other party.
    /// When accepted:
    /// - The original booking is updated with new details
    /// - The BookingRequest status changes to "Approved"
    /// 
    /// Validation checks:
    /// - Request must be in "Pending" status
    /// - Request type must be "CustomerUpdate" or "PtUpdate"
    /// - Target booking must exist
    /// </remarks>
    /// <response code="200">Edit request accepted and booking updated successfully</response>
    /// <response code="400">Invalid request (e.g., not pending, wrong type)</response>
    /// <response code="404">Booking request or target booking not found</response>
    [HttpPost("accept-edit-booking")]
    [ProducesResponseType(typeof(BaseResponse<UpdateBookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptEditBooking([FromBody] AcceptEditBookingRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<UpdateBookingResponseDto>(StatusCodes.Status200OK.ToString(), "Booking request accepted successfully", result));
    }
    /// <summary>
    /// Get all booking requests for a specific customer purchased package
    /// </summary>
    /// <param name="parameters">Query parameters including customer purchased ID and pagination</param>
    /// <returns>A paginated list of booking requests</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v1/bookings/booking-request?customerPurchasedId=3fa85f64-5717-4562-b3fc-2c963f66afa6&amp;page=1&amp;size=10
    ///     
    /// Returns booking requests with details including:
    /// - Request ID and type (CustomerCreate, PtCreate, CustomerUpdate, PtUpdate)
    /// - Request status (Pending, Approved, Rejected)
    /// - Customer and PT IDs
    /// - Booking name and notes
    /// - Target booking (if editing an existing booking)
    /// - Original booking details (for update requests)
    /// 
    /// Request types:
    /// - CustomerCreate: Customer initiated new booking request
    /// - PtCreate: PT initiated new booking request
    /// - CustomerUpdate: Customer proposed changes to existing booking
    /// - PtUpdate: PT proposed changes to existing booking
    /// </remarks>
    /// <response code="200">Booking requests retrieved successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Customer purchased package not found</response>
    [HttpGet("booking-request")]
    [ProducesResponseType(typeof(BaseResponse<Pagination<GetBookingRequestResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingRequest([FromQuery] GetBookingRequestParams parameters)
    {
        var result = await _mediator.Send(new GetBookingRequestQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetBookingRequestResponse>>(StatusCodes.Status200OK.ToString(), "Booking request retrieved successfully", pagination));
    }
}
