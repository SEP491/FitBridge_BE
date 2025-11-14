using FitBridge_Application.Dtos.Messaging;

namespace FitBridge_Application.Interfaces.Services.Messaging
{
    public interface IMessagingClient
    {
        // true means client acknoledged
        public Task MessageReceived(MessageReceivedDto message);

        public Task ReactionReceived(ReactionReceivedDto reaction);

        public Task ReactionRemoved(ReactionRemovedDto reaction);

        public Task UpdateMessageStatus(UpdateMessageStatusDto statusUpdate);

        public Task UserTyping(UserTypingDto typing);

        public Task UserPresenceUpdate(UserPresenceUpdateDto presenceUpdate);

        public Task ConversationUpdated(ConversationUpdatedDto conversationUpdate);

        public Task MessageUpdated(MessageUpdatedDto updatedMessage);

        //public Task<bool> Acknowledged(BaseDecorator acknowledgment);
    }
}