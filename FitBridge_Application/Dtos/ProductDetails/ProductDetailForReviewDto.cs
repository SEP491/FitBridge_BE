using System;

namespace FitBridge_Application.Dtos.ProductDetails;

public class ProductDetailForReviewDto
{
    public Guid Id { get; set; }
    public string ProductName { get; set; }
    public string FlavourName { get; set; }
    public string WeightUnit { get; set; }
    public double WeightValue { get; set; }
}
