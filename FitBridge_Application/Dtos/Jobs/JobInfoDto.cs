using System;

namespace FitBridge_Application.Dtos.Jobs;

public class JobInfoDto
{
    public string JobName { get; set; } = string.Empty;
    public string JobGroup { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TriggerState { get; set; } = string.Empty;
    public DateTime? NextFireTime { get; set; }
    public DateTime? PreviousFireTime { get; set; }
    public DateTime? StartTime { get; set; }
    public Dictionary<string, string> JobData { get; set; } = new();
}
