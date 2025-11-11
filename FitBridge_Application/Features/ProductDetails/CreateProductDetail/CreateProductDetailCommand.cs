using System;
using MediatR;

namespace FitBridge_Application.Features.ProductDetails.CreateProductDetail;

public class CreateProductDetailCommand : IRequest<string>
{
    public decimal OriginalPrice { get; set; }
    public decimal DisplayPrice { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public List<string> ImageUrls { get; set; } = new List<string>();
    public Guid ProductId { get; set; }
    public Guid WeightId { get; set; }
    public Guid FlavourId { get; set; }
    public int Quantity { get; set; }
    public bool IsDisplayed { get; set; }
}
