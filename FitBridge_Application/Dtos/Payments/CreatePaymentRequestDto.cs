using System;
using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.OrderItems;

namespace FitBridge_Application.Dtos.Payments;

public class CreatePaymentRequestDto
{
    [JsonIgnore]
    public Guid? AccountId { get; set; }
    [JsonIgnore]
    public decimal SubTotalPrice { get; set; }
    [JsonIgnore]
    public decimal TotalAmountPrice { get; set; }
    public Guid? CouponId { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? CustomerPurchasedIdToExtend { get; set; }
    public decimal ShippingFee { get; set; } = 0;
    public Guid? AddressId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    [JsonIgnore]
    public decimal? CommissionRate { get; set; }
}
