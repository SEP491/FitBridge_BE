using System;

namespace FitBridge_Application.Dtos.Orders;

public class ShippingEstimateDto
{
    public double Distance { get; set; }
    public double Duration { get; set; }
    public decimal TotalPay { get; set; }
}
