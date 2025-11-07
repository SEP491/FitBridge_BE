using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Notifications;

namespace FitBridge_Application.Dtos.Notifications
{
    public class NotificationDto(string additionalPayload, string notificationCategory)
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Body { get; set; }

        public long Timestamp { get; set; }

        public bool IsRead { get; set; }

        public string? NotificationCategory { get; set; } = notificationCategory;

        public NotificationTypes NotificationType { get; set; }

        public string? AdditionalPayload { get; set; } = additionalPayload;

        public NotificationDto() : this("", "")
        {
        }
    }
}