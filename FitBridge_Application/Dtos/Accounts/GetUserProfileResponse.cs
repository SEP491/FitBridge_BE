namespace FitBridge_Application.Dtos.Accounts;

public class GetUserProfileResponse
{
    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime? DOB { get; set; }

    public double? Weight { get; set; }

    public double? Height { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public string? IsActive { get; set; }
}