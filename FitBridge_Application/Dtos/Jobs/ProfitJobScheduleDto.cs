using System;

namespace FitBridge_Application.Dtos.Jobs;

public class ProfitJobScheduleDto
{
    public Guid CustomerPurchasedId { get; set; }
    public DateOnly ProfitDistributionDate { get; set; }
}
