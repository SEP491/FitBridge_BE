using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Message : BaseEntity
{
    public string Content { get; set; }

    public MessageType MessageType { get; set; }

    //        For User Messages:
    //•	Attachments: File information(name, size, type, URL)
    //•	Mentions: User IDs or usernames mentioned in the message
    //•	Formatting: Rich text formatting details
    //•	Links: Preview data for URLs shared in the message
    //•	Location: Geographic coordinates if sharing location
    //•	Voice/Video: Duration, codec information
    //For System Messages:
    //•	Event details: User joined/left, role changes, title updates
    //•	Action context: Which user performed the action, timestamp
    //•	Previous values: For audit trails(e.g., "title changed from X to Y")
    //General Use Cases:
    //•	Client information: Device type, app version that sent the message
    //•	Encryption: Key identifiers or encryption metadata
    //•	Analytics: Tracking or telemetry data
    //•	Custom flags: Priority, importance, categories
    public string? Metadata { get; set; }

    public MediaType MediaType { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Reaction { get; set; }

    public Guid ConversationId { get; set; }

    public Conversation Conversation { get; set; }

    public Guid? SenderId { get; set; }

    public ConversationMember? Sender { get; set; }

    public Guid? ReplyToMessageId { get; set; }

    public Message? ReplyToMessage { get; set; }

    public Guid? BookingRequestId { get; set; }

    public BookingRequest? BookingRequest { get; set; }

    public ICollection<MessageStatus> MessageStatuses { get; set; } = new List<MessageStatus>();
}