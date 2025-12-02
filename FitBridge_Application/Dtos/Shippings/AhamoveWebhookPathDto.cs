using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveWebhookPathDto
{
    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("short_address")]
    public string? ShortAddress { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }

    [JsonPropertyName("cod")]
    public decimal? Cod { get; set; }

    [JsonPropertyName("tracking_number")]
    public string? TrackingNumber { get; set; }

    [JsonPropertyName("fail_code")]
    public string? FailCode { get; set; }

    [JsonPropertyName("fail_comment")]
    public string? FailComment { get; set; }

    [JsonPropertyName("fail_time")]
    public double? FailTime { get; set; }

    [JsonPropertyName("complete_time")]
    public double? CompleteTime { get; set; }

    [JsonPropertyName("return_time")]
    public double? ReturnTime { get; set; }
}

