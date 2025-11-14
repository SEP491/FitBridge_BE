using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Messaging.GetMessages
{
    public class GetMessagesSpec : BaseSpecification<Message>
    {
        public GetMessagesSpec(
            Guid convoId,
            Guid? userId = null,
            GetMessagesParam? parameters = null,
            List<Guid>? messagesInclude = null,
            int? skip = null,
            int? take = null,
            Guid? messageId = null,
            bool includeOwnMessageStatus = false,
            bool includeReplyToMessage = false,
            bool includeBookingRequest = false,
            bool includeSender = false,
            bool includeConversationMembers = false) : base(x =>
            (messageId == null || x.Id == messageId)
            && x.ConversationId == convoId
            && (messagesInclude == null || messagesInclude.Contains(x.Id))
        )
        {
            if (parameters != null && parameters.DoApplyPaging)
            {
                AddOrderByDesc(x => x.CreatedAt);
                if (skip == null || take == null)
                {
                    skip = (parameters.Page - 1) * parameters.Size;
                    take = parameters.Size;
                }
                AddPaging(skip.Value, take.Value);
            }
            if (includeOwnMessageStatus)
            {
                AddInclude(x => x.MessageStatuses.Where(x => x.UserId == userId));
            }
            if (includeConversationMembers)
            {
                AddInclude(x => x.Conversation.ConversationMembers);
                AddInclude($"{nameof(Message.Conversation)}.{nameof(Conversation.ConversationMembers)}.{nameof(ConversationMember.User)}");
            }
            if (includeReplyToMessage)
            {
                AddInclude(x => x.ReplyToMessage);
            }
            if (includeSender)
            {
                AddInclude(x => x.Sender);
                AddInclude($"{nameof(Message.Sender)}.{nameof(ConversationMember.User)}");
            }
            if (includeBookingRequest)
            {
                AddInclude(x => x.BookingRequest);
            }
        }
    }
}