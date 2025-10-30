using System;
using FitBridge_Domain.Entities.Accounts;

namespace FitBridge_Application.Dtos.Accounts.FreelancePts;

public class GetAllFreelancePTsResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string AvatarUrl { get; set; }
    public string Description { get; set; }
    public double Rating { get; set; }
    public List<string> GoalTrainings { get; set; }
    public List<string> Certifications { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public decimal PriceFrom { get; set; }
    public int ExperienceYears { get; set; }
    public int TotalPurchased { get; set; }
}
