using System;

namespace FitBridge_Application.Dtos.CustomerPurchaseds.TrainingResults;

public class HighestPerformanceDto
{
    public double TotalWeight { get; set; }
    public DateOnly Date { get; set; }
    public string? SessionName { get; set; }
}
