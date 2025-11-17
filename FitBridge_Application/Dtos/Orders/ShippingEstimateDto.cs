using System;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Orders;

public class ShippingEstimateDto
{
    [JsonPropertyName("distance")]
    public double distance { get; set; }
    [JsonPropertyName("duration")]
    public double duration { get; set; }
    [JsonPropertyName("total_pay")]
    public decimal totalPay { get; set; }
}
