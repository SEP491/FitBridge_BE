using System;
using System.Text.Json.Serialization;
using FitBridge_Application.Dtos.OrderItems;
using MediatR;
namespace FitBridge_Application.Features.Orders.CreateOrders;

public class CreateOrderCommand : IRequest<string>
{
    [JsonIgnore]
    public Guid? AccountId { get; set; }
    public Guid? VoucherId { get; set; }
    public decimal ShippingFee { get; set; } = 0;
    public Guid? AddressId { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}
