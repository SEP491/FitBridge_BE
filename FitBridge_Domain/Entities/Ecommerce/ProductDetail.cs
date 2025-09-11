using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Ecommerce;

public class ProductDetail : BaseEntity
{
    public decimal OriginalPrice { get; set; }
    public decimal DisplayPrice { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid ProductId { get; set; }
    public Guid WeightId { get; set; }
    public Guid FlavourId { get; set; }
    public Product Product { get; set; }
    public Weight Weight { get; set; }
    public Flavour Flavour { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
