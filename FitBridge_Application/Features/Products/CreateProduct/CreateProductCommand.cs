using System;
using MediatR;

namespace FitBridge_Application.Features.Products.CreateProduct;

public class CreateProductCommand : IRequest<string>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double Volume { get; set; }
    public Guid BrandId { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public Guid SubCategoryId { get; set; }
    public bool IsDisplayed { get; set; }
}
