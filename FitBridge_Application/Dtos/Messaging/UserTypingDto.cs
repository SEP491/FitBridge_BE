namespace FitBridge_Application.Dtos.Messaging
{
    public class UserTypingDto : IMessagingHubDto
    {
        public Guid TyperId { get; set; }

        public string TyperName { get; set; }

        public bool IsTyping { get; set; }

        public Guid ConversationId { get; set; }
    }
}