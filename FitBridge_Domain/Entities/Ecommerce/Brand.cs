using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Ecommerce;

public class Brand : BaseEntity
{
    public string Name { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
