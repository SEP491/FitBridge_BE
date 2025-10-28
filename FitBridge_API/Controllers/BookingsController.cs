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
using FitBridge_Application.Dtos.GymSlots;
using FitBridge_Application.Specifications.Bookings.GetGymSlotForBooking;
using FitBridge_Application.Features.Bookings.GetGymSlotForBooking;
using FitBridge_Application.Specifications.Bookings.GetFreelancePtSchedule;
using FitBridge_Application.Features.Bookings.GetFreelancePtSchedule;
using FitBridge_Application.Features.Bookings.GetGymPtSchedule;
using FitBridge_Application.Specifications.Bookings.GetGymPtSchedule;
using FitBridge_Application.Features.Bookings.AcceptBookingRequestCommand;
using FitBridge_Application.Features.Bookings.RequestEditBooking;
using FitBridge_Application.Features.Bookings.AcceptEditBookingRequest;
using FitBridge_Application.Specifications.Bookings.GetBookingRequests;
using FitBridge_Application.Features.Bookings.GetBookingRequest;
using FitBridge_Application.Features.Bookings.RejectBookingRequest;
using FitBridge_Application.Features.Bookings.GetTrainingResult;
using FitBridge_Application.Features.Bookings.CreateBooking;
using FitBridge_Application.Features.Bookings.StartBookingSession;
using FitBridge_Application.Features.Bookings.EndBookingSession;
using FitBridge_Application.Features.Bookings.GetBookingHistory;
using FitBridge_Application.Specifications.Bookings.GetBookingHistory;

namespace FitBridge_API.Controllers;

public class BookingsController(IMediator _mediator) : _BaseApiController
{
    /// <summary>
    /// Retrieves the booking history for the current logged-in user (Customer, Gym PT, or Freelance PT).
    /// </summary>
    /// <param name="parameters">Query parameters for filtering and pagination, including:
    /// <list type="bullet">
    /// <item>
    /// <term>StartDate</term>
    /// <description>Filter bookings from this date onwards (optional).</description>
    /// </item>
    /// <item>
    /// <term>EndDate</term>
    /// <description>Filter bookings up to this date (optional).</description>
    /// </item>
    /// <item>
    /// <term>Status</term>
    /// <description>Filter by booking status (Booked, InProgress, Finished, Cancelled) (optional).</description>
    /// </item>
    /// <item>
    /// <term>SearchTerm</term>
    /// <description>Search in booking name or notes (optional).</description>
    /// </item>
    /// <item>
    /// <term>Page</term>
    /// <description>The page number to retrieve (default: 1).</description>
    /// </item>
    /// <item>
    /// <term>Size</term>
    /// <description>The number of items per page (default: 10, max: 20).</description>
    /// </item>
    /// <item>
    /// <term>SortBy</term>
    /// <description>The field to sort by (BookingDate, Status, CreatedAt).</description>
    /// </item>
    /// <item>
    /// <term>SortOrder</term>
    /// <description>The sort direction (asc or desc).</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns>A paginated list of booking history for the current user.</returns>
    /// <remarks>
    /// The response includes different information based on user role:
    /// - For Customers: Shows PT name, avatar, and gym slot information
    /// - For Freelance PTs: Shows customer name, avatar, and package information
    /// - For Gym PTs: Shows customer name, avatar, and gym slot information
    ///
    /// Sample request:
    ///
    ///     GET /api/v1/bookings/history?startDate=2025-01-01&amp;endDate=2025-12-31&amp;status=Finished&amp;page=1&amp;size=10
    ///
    /// </remarks>
    /// <response code="200">Booking history retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("history")]
    [Authorize(Roles = ProjectConstant.UserRoles.Customer + "," + ProjectConstant.UserRoles.GymPT + "," + ProjectConstant.UserRoles.FreelancePT)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetBookingHistoryResponseDto>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<GetBookingHistoryResponseDto>>> GetBookingHistory(
        [FromQuery] GetBookingHistoryParams parameters)
    {
        var result = await _mediator.Send(new GetBookingHistoryQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);

        return Ok(new BaseResponse<Pagination<GetBookingHistoryResponseDto>>(
            StatusCodes.Status200OK.ToString(),
            "Booking history retrieved successfully",
            pagination));
    }

    [HttpPost("cancel-booking")]
    [Authorize(Roles = ProjectConstant.UserRoles.Customer + "," + ProjectConstant.UserRoles.GymPT + "," + ProjectConstant.UserRoles.FreelancePT)]
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
    /// Get gym PT's booking schedule for a specific date showing all bookings with customer details
    /// </summary>
    /// <param name="parameters">Query parameters including date and pagination</param>
    /// <returns>Paginated list of gym PT bookings</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/v1/bookings/gym-pt-schedule?date=2025-01-15&amp;page=1&amp;size=10
    ///
    /// Returns bookings for the authenticated gym PT including:
    /// - Booking details (ID, name, date, time slot)
    /// - Customer information (name, avatar)
    /// - Course name
    /// - Gym slot details
    /// - Session status and notes
    ///
    /// Only accessible by gym PTs to view their daily schedule.
    /// </remarks>
    /// <response code="200">Schedule retrieved successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">PT not found</response>
    [HttpGet("gym-pt-schedule")]
    [Authorize(Roles = ProjectConstant.UserRoles.GymPT)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<Pagination<GetGymPtScheduleResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGymPtSchedule([FromQuery] GetGymPtScheduleParams parameters)
    {
        var result = await _mediator.Send(new GetGymPtScheduleQuery { Params = parameters });
        var pagination = ResultWithPagination(result.Items, result.Total, parameters.Page, parameters.Size);
        return Ok(new BaseResponse<Pagination<GetGymPtScheduleResponse>>(StatusCodes.Status200OK.ToString(), "Schedule retrieved successfully", pagination));
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
    ///       "bookingRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
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

    /// <summary>
    /// Reject a booking request for customer and freelance pt
    /// </summary>
    /// <param name="command">The command containing the booking request ID to reject</param>
    /// <returns>The ID of the rejected booking request</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1/bookings/reject-booking-request
    ///     {
    ///         "bookingRequestId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    ///
    /// Validation checks:
    /// - Request must be in "Pending" status
    /// - Target booking must exist
    /// </remarks>
    [Authorize(Roles = ProjectConstant.UserRoles.Customer + "," + ProjectConstant.UserRoles.FreelancePT)]
    [HttpPost("reject-booking-request")]
    public async Task<IActionResult> RejectBookingRequest([FromBody] RejectBookingRequestCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<bool>(StatusCodes.Status200OK.ToString(), "Booking request rejected successfully", result));
    }

    /// <summary>
    /// API for get training result of a booking
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("result/{id}")]
    public async Task<IActionResult> GetTrainingResult([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetTrainingResultQuery { BookingId = id });
        return Ok(new BaseResponse<SessionReportDto>(StatusCodes.Status200OK.ToString(), "Booking result retrieved successfully", result));
    }

    /// <summary>
    /// Start a booking session, update booking session start time
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("start-booking-session")]
    public async Task<IActionResult> StartBookingSession([FromBody] StartBookingSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<DateTime>(StatusCodes.Status200OK.ToString(), "Booking session started successfully", result));
    }

    /// <summary>
    /// End a booking session, update booking status to finished and set session end time
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("end-booking-session")]
    public async Task<IActionResult> EndBookingSession([FromBody] EndBookingSessionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(new BaseResponse<DateTime>(StatusCodes.Status200OK.ToString(), "Booking session ended successfully", result));
    }
}