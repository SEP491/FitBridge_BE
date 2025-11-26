using System;
using FitBridge_Application.Dtos.ProductDetails;

namespace FitBridge_Application.Dtos.OrderItems;

public class OrderItemForProductOrderResponseDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool IsFeedback { get; set; }
    public Guid OrderId { get; set; }
    public Guid? ProductDetailId { get; set; }
    public ProductDetailForAdminResponseDto ProductDetail { get; set; }
    public decimal? OriginalProductPrice { get; set; }
    public bool IsRefunded { get; set; }
}
