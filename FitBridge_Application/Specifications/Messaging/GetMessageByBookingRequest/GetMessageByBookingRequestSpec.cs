using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Messaging.GetMessageByBookingRequest
{
    public class GetMessageByBookingRequestSpec : BaseSpecification<Message>
    {
        public GetMessageByBookingRequestSpec(Guid bookingRequestId) : base(x =>
            x.BookingRequestId == bookingRequestId)
        {
            AddInclude(x => x.Conversation);
        }
    }
}