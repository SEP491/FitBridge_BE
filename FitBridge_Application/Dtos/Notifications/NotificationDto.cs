using FitBridge_Domain.Enums.Notifications;

namespace FitBridge_Application.Dtos.Notifications
{
    public class NotificationDto(Dictionary<string, string>? additionalPayload)
    {
        public string? Title { get; set; }

        public string? Body { get; set; }

        public DateTime Timestamp { get; set; }

        public bool IsRead { get; set; }

        public NotificationTypes NotificationType { get; set; }

        public Dictionary<string, string>? AdditionalPayload { get; set; } = additionalPayload;
    }
}