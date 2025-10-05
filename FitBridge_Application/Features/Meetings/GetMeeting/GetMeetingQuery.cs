using FitBridge_Application.Dtos.Meetings;
using MediatR;

namespace FitBridge_Application.Features.Meetings.GetMeeting
{
    public class GetMeetingQuery : IRequest<GetMeetingDto>
    {
        public Guid BookingId { get; set; }
    }
}