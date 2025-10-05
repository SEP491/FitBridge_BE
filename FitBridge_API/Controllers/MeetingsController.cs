using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos.Meetings;
using FitBridge_Application.Features.Meetings.CreateMeeting;
using FitBridge_Application.Features.Meetings.GetMeeting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
    /// <summary>
    /// Controller for managing meetings.
    /// </summary>
    [Authorize]
    public class MeetingsController(IMediator mediator) : _BaseApiController
    {
        /// <summary>
        /// Creates a new meeting for a given booking.
        /// </summary>
        /// <param name="command">The command containing the booking ID.</param>
        /// <returns>The created meeting details.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMeeting([FromBody] CreateMeetingCommand command)
        {
            var result = await mediator.Send(command);
            return Created(
                nameof(CreateMeeting),
                new BaseResponse<CreateMeetingDto>(
                    StatusCodes.Status201Created.ToString(),
                    "Meeting created successfully",
                    result));
        }

        /// <summary>
        /// Retrieves the meeting details for a given booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking.</param>
        /// <returns>The meeting details.</returns>
        [HttpGet("{bookingId:guid}")]
        public async Task<IActionResult> GetMeeting(Guid bookingId)
        {
            var query = new GetMeetingQuery { BookingId = bookingId };
            var result = await mediator.Send(query);
            return Ok(
                new BaseResponse<GetMeetingDto>(
                    StatusCodes.Status200OK.ToString(),
                    "Meeting retrieved successfully",
                    result));
        }
    }
}