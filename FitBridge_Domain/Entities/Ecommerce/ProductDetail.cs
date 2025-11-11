using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Domain.Entities.Ecommerce;

public class ProductDetail : BaseEntity
{
    public decimal OriginalPrice { get; set; }
    public decimal DisplayPrice { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public Guid ProductId { get; set; }
    public Guid WeightId { get; set; }
    public Guid FlavourId { get; set; }
    public int Quantity { get; set; }
    public int SoldQuantity { get; set; }
    public bool IsDisplayed { get; set; }
    public Product Product { get; set; }
    public Weight Weight { get; set; }
    public Flavour Flavour { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
