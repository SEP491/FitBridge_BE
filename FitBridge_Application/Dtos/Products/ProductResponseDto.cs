using System;

namespace FitBridge_Application.Dtos.Products;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string BrandName { get; set; }
    public string SubCategoryName { get; set; }
    public string CoverImageUrl { get; set; }
    public bool IsDisplayed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
