using System;

namespace FitBridge_Application.Configurations;

public class EmailSettings
{
    public const string SectionName = "Smtp";
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
}
