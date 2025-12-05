using System;

namespace FitBridge_Application.Dtos.FreelancePTPackages;

public class FreelancePtPackageSummaryDto
{
    public int TotalPackages { get; set; }
    public decimal TotalPrices { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal AvgSessions { get; set; }
    public int PtMaxCourse { get; set; }
    public int PtCurrentCourse { get; set; }
}   
