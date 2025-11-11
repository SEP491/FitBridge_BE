using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace FitBridge_Application.Features.ProductDetails.UpdateProductDetail;

public class UpdateProductDetailCommand : IRequest<string>
{
    [JsonIgnore]
    public Guid? Id { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DisplayPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public DateOnly? ExpirationDate { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? WeightId { get; set; }
    public Guid? FlavourId { get; set; }
    public int? Quantity { get; set; }
    public bool? IsDisplayed { get; set; }
    public string? ServingSizeInformation { get; set; }
    public string? ServingsPerContainerInformation { get; set; }
    public double? ProteinPerServingGrams { get; set; }
    public int? CaloriesPerServingKcal { get; set; }
    public int? BCAAPerServingGrams { get; set; }
}
