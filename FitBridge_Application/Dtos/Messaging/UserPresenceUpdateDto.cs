namespace FitBridge_Application.Dtos.Messaging
{
    public class UserPresenceUpdateDto : IMessagingHubDto
    {
        //public Guid ConversationId { get; set; }

        public bool IsOnline { get; set; }
    }
}