using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Specifications.Messaging.GetMessages;
using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Services
{
    internal class MessagingService(IUnitOfWork unitOfWork)
    {
        internal static readonly int MESSAGE_NOT_FOUND = -1;

        public async Task<int?> GetMessageIndexAsync(Guid convoId, Guid messageId)
        {
            var spec = new GetMessagesSpec(convoId, messageId);
            var targetMessage = await unitOfWork
                .Repository<Message>()
                .GetBySpecificationAsync(spec);

            if (targetMessage == null)
            {
                return MESSAGE_NOT_FOUND;
            }
            var countSpec = new GetMessagesAfterAMessageSpec(convoId, targetMessage.CreatedAt);
            var index = await unitOfWork.Repository<Message>()
                    .CountAsync(spec);
            if (index < -1) return MESSAGE_NOT_FOUND;

            return index + 1; // 1-based index
        }
    }
}