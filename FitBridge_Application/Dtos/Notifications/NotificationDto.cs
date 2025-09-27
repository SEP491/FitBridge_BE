namespace FitBridge_Application.Dtos.Notifications
{
    public class NotificationDto(Dictionary<string, string>? additionalPayload)
    {
        public string? Title { get; set; }

        public string? Body { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Dictionary<string, string>? AdditionalPayload { get; set; } = additionalPayload;
    }
}