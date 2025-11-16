 using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveOrderResponseDto
{
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("shared_link")]
    public string SharedLink { get; set; }

    [JsonPropertyName("order")]
    public AhamoveOrderDetailDto Order { get; set; }
}

