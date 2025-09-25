using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Configurations
{
    public class NotificationSettings
    {
        public const string SectionName = "Notifications";

        [Required]
        public int MaxHandshakeRetries { get; set; } = 3;

        [Required]
        public int InitialRetryDelayMs { get; set; } = 5000;
    }
}