using System;
using FitBridge_Application.Dtos.Orders;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Shippings;

public class ShippingEstimateWrapper
{
    [JsonPropertyName("service_id")]
    public string serviceId { get; set; }
    [JsonPropertyName("data")]
    public ShippingEstimateDto data { get; set; }
    [JsonPropertyName("error")]
    public string? error { get; set; }
}
