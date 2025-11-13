namespace FitBridge_Application.Dtos.Messaging
{
    public class ReactionRemovedDto : IMessagingHubDto
    {
        public Guid MessageId { get; set; }
    }
}