using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveWebhookDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("sub_status")]
    public string? SubStatus { get; set; }

    [JsonPropertyName("accept_time")]
    public double? AcceptTime { get; set; }

    [JsonPropertyName("board_time")]
    public double? BoardTime { get; set; }

    [JsonPropertyName("pickup_time")]
    public double? PickupTime { get; set; }

    [JsonPropertyName("complete_time")]
    public double? CompleteTime { get; set; }

    [JsonPropertyName("cancel_time")]
    public double? CancelTime { get; set; }

    [JsonPropertyName("cancel_by_user")]
    public bool? CancelByUser { get; set; }

    [JsonPropertyName("cancel_code")]
    public string? CancelCode { get; set; }

    [JsonPropertyName("cancel_comment")]
    public string? CancelComment { get; set; }

    [JsonPropertyName("rebroadcast_comment")]
    public string? RebroadcastComment { get; set; }

    [JsonPropertyName("path")]
    public List<AhamoveWebhookPathDto> Path { get; set; }

    [JsonPropertyName("supplier_id")]
    public string? SupplierId { get; set; }

    [JsonPropertyName("supplier_name")]
    public string? SupplierName { get; set; }

    [JsonPropertyName("service_id")]
    public string ServiceId { get; set; }

    [JsonPropertyName("payment_method")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("total_pay")]
    public decimal TotalPay { get; set; }

    [JsonPropertyName("city_id")]
    public string CityId { get; set; }

    [JsonPropertyName("create_time")]
    public double CreateTime { get; set; }

    [JsonPropertyName("order_time")]
    public double OrderTime { get; set; }

    [JsonPropertyName("shared_link")]
    public string? SharedLink { get; set; }
}

