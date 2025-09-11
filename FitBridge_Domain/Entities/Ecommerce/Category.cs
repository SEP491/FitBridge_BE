using System;

namespace FitBridge_Domain.Entities.Ecommerce;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}
