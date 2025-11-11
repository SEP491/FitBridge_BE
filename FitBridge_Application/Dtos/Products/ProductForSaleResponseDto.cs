using System;

namespace FitBridge_Application.Dtos.Products;

public class ProductForSaleResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal DisplayPrice { get; set; }
    public decimal SalePrice { get; set; }
    public int Quantity { get; set; }
    public int TotalSoldQuantity { get; set; }
    public string ImageUrl { get; set; }
    public decimal PriceFrom { get; set; }
    public double Rating { get; set; }
    public int TotalReviews { get; set; }
    public string? CountryOfOrigin { get; set; }
}
