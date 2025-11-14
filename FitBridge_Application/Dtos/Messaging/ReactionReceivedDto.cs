namespace FitBridge_Application.Dtos.Messaging
{
    public class ReactionReceivedDto : IMessagingHubDto
    {
        public string Reaction { get; set; }

        public Guid MessageId { get; set; }
    }
}