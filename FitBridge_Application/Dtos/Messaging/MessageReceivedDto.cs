namespace FitBridge_Application.Dtos.Messaging
{
    public class MessageReceivedDto : IMessagingHubDto
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string MessageType { get; set; } // system, user

        public string MediaType { get; set; } // text, image, video, audio, file

        public string? Metadata { get; set; }

        public Guid ConversationId { get; set; }

        public Guid SenderId { get; set; }

        public string SenderName { get; set; }

        public string? SenderAvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } //date the msg is created

        public Guid? ReplyToMessageId { get; set; }

        public string? ReplyToMessageContent { get; set; }

        public string? ReplyToMessageMediaType { get; set; }

        public NewConversationDto? NewConversation { get; set; } = null;

        public BookingRequestDto? BookingRequest { get; set; } = null;
    }

    public class NewConversationDto
    {
        public string ConversationImg { get; set; }

        public bool IsGroup { get; set; }
    }
}