using System;

namespace FitBridge_Application.Dtos.Emails;

public class AccountInformationEmailData
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public bool IsMale { get; set; }
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? GymName { get; set; }
    public string? TaxCode { get; set; }
    public string? ConfirmationLink { get; set; }
    public string? EmailType { get; set; }
}
