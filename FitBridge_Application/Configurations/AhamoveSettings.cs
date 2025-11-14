namespace FitBridge_Application.Configurations;

public class AhamoveSettings
{
    public const string SectionName = "Ahamove";
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; } = "https://partner-apistg.ahamove.com";
}

