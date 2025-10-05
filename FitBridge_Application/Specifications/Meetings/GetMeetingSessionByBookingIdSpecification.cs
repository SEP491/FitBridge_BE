using FitBridge_Domain.Entities.Meetings;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Meetings
{
    public class GetMeetingSessionByBookingIdSpecification : BaseSpecification<MeetingSession>
    {
        public GetMeetingSessionByBookingIdSpecification(Guid bookingId) : base(x =>
            x.IsEnabled)
        {
        }
    }
}