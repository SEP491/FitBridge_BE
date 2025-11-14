namespace FitBridge_Application.Dtos.Messaging
{
    public class MessageUpdatedDto : IMessagingHubDto
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public string? NewContent { get; set; }

        public string Status { get; set; } //updated, deleted

        public string MessageType { get; set; } // system, user

        public BookingRequestDto? BookingRequest { get; set; } = null;
    }
}