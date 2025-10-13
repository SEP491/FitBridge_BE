using System;

namespace FitBridge_Application.Configurations;

public class AppWriteSettings
{
    public const string SectionName = "AppWrite";
    public string EndPoint { get; set; }
    public string ProjectId { get; set; }
    public string APIKey { get; set; }
    public string Bucket { get; set; }
}
