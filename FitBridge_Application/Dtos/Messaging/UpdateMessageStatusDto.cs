namespace FitBridge_Application.Dtos.Messaging
{
    public class UpdateMessageStatusDto : IMessagingHubDto
    {
        public string UpdatedStatus { get; set; } // sent, delivered, read

        public DateTime Timestamp { get; set; }
    }
}