using System;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Dtos.Shippings;

public class AhamoveServiceDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }
}
