using System.ComponentModel.DataAnnotations;

namespace FitBridge_Application.Configurations
{
    public class MeetingSettings
    {
        public const string SectionName = "Meeting";

        [Required]
        public int ShowMeetingAlertTime { get; set; } = 10; // 2h5m in seconds, 10s for testing

        [Required]
        public int StopMeetingTime { get; set; } = 15; // 2h10m in seconds, 15s for testing
    }
}