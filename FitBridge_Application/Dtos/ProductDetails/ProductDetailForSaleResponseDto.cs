using System;
using FitBridge_Application.Dtos.Reviews;

namespace FitBridge_Application.Dtos.ProductDetails;

public class ProductDetailForSaleResponseDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductImageUrl { get; set; }
    public double? Rating { get; set; }
    public string BrandName { get; set; }
    public string SubCategoryName { get; set; }
    public string ProteinSources { get; set; }
    public string CountryOfOrigin { get; set; }
    public string Description { get; set; }
    public List<ProductDetailForAdminResponseDto> ProductDetails { get; set; }
    public List<ReviewProductResponseDto>? Reviews { get; set; }
}
