using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace FitBridge_Application.Features.Products.CreateProduct;

public class CreateProductCommand : IRequest<string>
{
    public IFormFile? CoverImage { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid BrandId { get; set; }
    public string? ProteinSources { get; set; }
    public string? CountryOfOrigin { get; set; }
    public Guid SubCategoryId { get; set; }
    public bool IsDisplayed { get; set; }
}
