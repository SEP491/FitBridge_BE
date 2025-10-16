using System;

namespace FitBridge_Application.Dtos.Accounts.HotResearch;

public class HotResearchAccountDto
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool HotResearch { get; set; }
    public string? GymName { get; set; }
    public string UserRole { get; set; } = string.Empty;
    
}
