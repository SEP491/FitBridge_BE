using System;

namespace FitBridge_Application.Dtos.Shippings;

public class AhamovePriceEstimateDto
{
    public long OrderTime { get; set; }
    public List<AhamovePathDto> Path { get; set; }
    public List<AhamoveServiceDto> Services { get; set; }
    public string PaymentMethod { get; set; }
}
