using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Domain.Entities.Orders;

public class OrderItem : BaseEntity
{
    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsFeedback { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; }

    public Guid ProductDetailId { get; set; }

    public ProductDetail ProductDetail { get; set; }
}