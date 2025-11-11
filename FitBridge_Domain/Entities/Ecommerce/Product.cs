using System;

namespace FitBridge_Domain.Entities.Ecommerce;

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Volume { get; set; }
    public Guid BrandId { get; set; }
    public Brand Brand { get; set; }
    public string CoverImageUrl { get; set; }
    public Guid SubCategoryId { get; set; }
    public bool IsDisplayed { get; set; }
    public SubCategory SubCategory { get; set; }
    public ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
