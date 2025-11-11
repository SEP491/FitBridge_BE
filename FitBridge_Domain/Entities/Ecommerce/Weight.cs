using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Ecommerce;

public class Weight : BaseEntity
{
    public double Value { get; set; }
    public string Unit { get; set; }
    public ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
