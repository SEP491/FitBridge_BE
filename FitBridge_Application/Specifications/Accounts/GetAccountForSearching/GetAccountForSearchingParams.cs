using System;
using FitBridge_Domain.Enums.ApplicationUser;

namespace FitBridge_Application.Specifications.Accounts.GetAccountForSearching;

public class GetAccountForSearchingParams : BaseParams
{
    public string? SearchTerm { get; set; }
    public SearchType SearchType { get; set; } = SearchType.FreelancePTAndGym;
    public decimal FromPrice { get; set; } = 0;
    public decimal ToPrice { get; set; } = 0;
    public double Rating { get; set; } = 0;
    public int ExperienceYears { get; set; } = 0;
    public List<string>? GoalTrainings { get; set; }
}
