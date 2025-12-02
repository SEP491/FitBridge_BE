using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using FitBridge_Application.Dtos.Products;
using MediatR;

namespace FitBridge_Application.Features.Products.UpdateProduct;

public class UpdateProductCommand : IRequest<ProductResponseDto>
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IFormFile? CoverImage { get; set; }
    public Guid? BrandId { get; set; }
    public string? ProteinSources { get; set; }
    public string? CountryOfOrigin { get; set; }
    public Guid? SubCategoryId { get; set; }
    public bool? IsDisplayed { get; set; }
}
