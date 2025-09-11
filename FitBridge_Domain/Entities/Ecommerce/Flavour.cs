using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Ecommerce;

public class Flavour : BaseEntity
{
    public string Name { get; set; }
    public ICollection<ProductDetail> ProductDetails { get; set; } = new List<ProductDetail>();
}
