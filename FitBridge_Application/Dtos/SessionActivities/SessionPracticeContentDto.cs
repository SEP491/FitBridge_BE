using System;

namespace FitBridge_Application.Dtos.SessionActivities;

public class SessionPracticeContentDto
{
    public Guid BookingId { get; set; }
    public string note { get; set; }
    public string NutritionTip { get; set; }
    public string BookingName { get; set; }
    public DateTime? SessionStartTime { get; set; }
    public DateTime? SessionEndTime { get; set; }
    public List<SessionActivityListDto> SessionActivities { get; set; } = new List<SessionActivityListDto>();
}
