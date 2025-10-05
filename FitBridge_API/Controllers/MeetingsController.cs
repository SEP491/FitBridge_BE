using FitBridge_Application.Features.Meetings.CreateMeeting;
using FitBridge_Application.Features.Meetings.GetMeeting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitBridge_API.Controllers
{
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
            return CreatedAtAction(nameof(GetMeeting), new { bookingId = command.BookingId }, result);
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
            return Ok(result);
        }
    }
}