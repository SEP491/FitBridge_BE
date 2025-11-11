using System;
using FitBridge_Application.Dtos.ProductDetails;

namespace FitBridge_Application.Dtos.Products;

public class ProductByIdResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string BrandName { get; set; }
    public Guid BrandId { get; set; }
    public Guid SubCategoryId { get; set; }
    public string SubCategoryName { get; set; }
    public string CoverImageUrl { get; set; }
    public bool IsDisplayed { get; set; }
    public bool TotalSold { get; set; }
    public List<ProductDetailForAdminResponseDto> ProductDetails { get; set; } = new List<ProductDetailForAdminResponseDto>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
