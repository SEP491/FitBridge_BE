using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Messaging.GetMessages
{
    public class GetMessagesAfterAMessageSpec : BaseSpecification<Message>
    {
        public GetMessagesAfterAMessageSpec(Guid convoId, DateTime msgCreatedAt) : base(x =>
            x.ConversationId == convoId
            && x.CreatedAt > msgCreatedAt)
        {
        }
    }
}