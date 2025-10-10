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

        // iOS-specific settings
        [Required]
        public string IOS_BundleId { get; set; } = string.Empty;

        [Required]
        public string IOS_CertificatePath { get; set; } = string.Empty;

        [Required]
        public string IOS_KeyId { get; set; } = string.Empty;

        [Required]
        public string IOS_TeamId { get; set; } = string.Empty;
    }
}