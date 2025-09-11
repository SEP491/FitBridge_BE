using System;
using FitBridge_Domain.Entities;

namespace FitBridge_Domain.Entities.Ecommerce;

public class SubCategory : BaseEntity
{
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
