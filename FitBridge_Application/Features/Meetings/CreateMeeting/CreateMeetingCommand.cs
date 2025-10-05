using FitBridge_Application.Dtos.Meetings;
using MediatR;

namespace FitBridge_Application.Features.Meetings.CreateMeeting
{
    public class CreateMeetingCommand : IRequest<CreateMeetingDto>
    {
        public Guid BookingId { get; set; }
    }
}