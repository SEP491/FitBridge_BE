namespace FitBridge_Application.Dtos.Accounts;

public class GetUserProfileResponse
{
    public Guid? Id { get; set; }
    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime? DOB { get; set; }

    public double? Weight { get; set; }

    public double? Height { get; set; }

    public string? Gender { get; set; }

    public string? AvatarUrl { get; set; }
    public DateOnly? GymFoundationDate { get; set; }
    public string? TaxCode { get; set; }
    public string? Bio { get; set; }
    public string? GymDescription { get; set; }
    public string? GymName { get; set; }

    public string? IsActive { get; set; }

    public string? FrontCitizenIdUrl { get; set; }
    public string? BackCitizenIdUrl { get; set; }
    public string? CitizenIdNumber { get; set; }
    public string? IdentityCardPlace { get; set; }
    public string? CitizenCardPermanentAddress { get; set; }
    public DateOnly? IdentityCardDate { get; set; }
    public string? BusinessAddress { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public int? PtMaxCourse { get; set; }
    public int? PtCurrentCourse { get; set; }
    public List<string> GymImages { get; set; }
    public List<string> FreelancePtImages{ get; set; }
}