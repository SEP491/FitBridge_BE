using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Messaging.GetMessageStatuses
{
    public class GetMessageStatusesSpec : BaseSpecification<MessageStatus>
    {
        public GetMessageStatusesSpec(Guid msgId, Guid userId) : base(x =>
            x.MessageId == msgId
            && x.UserId == userId)
        {
        }
    }
}