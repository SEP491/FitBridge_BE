using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveOrderDetailDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }

    [JsonPropertyName("total_fee")]
    public decimal TotalFee { get; set; }

    [JsonPropertyName("total_price")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("distance_price")]
    public decimal DistancePrice { get; set; }

    [JsonPropertyName("special_request_price")]
    public decimal SpecialRequestPrice { get; set; }

    [JsonPropertyName("service_id")]
    public string ServiceId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("distance")]
    public decimal Distance { get; set; }

    [JsonPropertyName("duration")]
    public int Duration { get; set; }
}

