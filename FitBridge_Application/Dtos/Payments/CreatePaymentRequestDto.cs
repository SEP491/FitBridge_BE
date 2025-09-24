using System;
using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.OrderItems;

namespace FitBridge_Application.Dtos.Payments;

public class CreatePaymentRequestDto
{
    [JsonIgnore]
    public Guid? AccountId { get; set; }
    [JsonIgnore]
    public decimal TotalAmount { get; set; }
    public Guid? VoucherId { get; set; }
    public decimal ShippingFee { get; set; } = 0;
    public Guid? AddressId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}
