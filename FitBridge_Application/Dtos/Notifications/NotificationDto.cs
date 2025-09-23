namespace FitBridge_Application.Dtos.Notifications
{
    public class NotificationDto(int id, string content, Dictionary<string, string>? additionalPayload, DateTime timestamp)
    {
        public int Id { get; set; } = id;

        public string Content { get; set; } = content;

        public DateTime Timestamp { get; set; } = timestamp;

        public bool IsRead { get; set; } = false; // whether the notification has been read by the user

        public Dictionary<string, string>? AdditionalPayload { get; set; } = additionalPayload;

        public string Type { get; set; }

        public string? Title { get; set; }
    }
}