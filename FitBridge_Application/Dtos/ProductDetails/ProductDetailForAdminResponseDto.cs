using System;

namespace FitBridge_Application.Dtos.ProductDetails;

public class ProductDetailForAdminResponseDto
{
    public Guid Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string WeightUnit { get; set; }
    public double WeightValue { get; set; }
    public string FlavourName { get; set; }
}
