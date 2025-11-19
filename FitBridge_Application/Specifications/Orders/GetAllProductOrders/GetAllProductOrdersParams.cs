using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Orders.GetAllProductOrders;

public class GetAllProductOrdersParams : BaseParams
{
    public Guid? CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? FromTime { get; set; }
    public DateTime? ToTime { get; set; }
    public string? ShippingTrackingId { get; set; }
}
