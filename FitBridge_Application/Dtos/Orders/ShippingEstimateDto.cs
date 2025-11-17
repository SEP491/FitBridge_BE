using System;

namespace FitBridge_Application.Dtos.Orders;

public class ShippingEstimateDto
{
    public double distance { get; set; }
    public double duration { get; set; }
    public decimal totalPay { get; set; }
}
