namespace FitBridge_Application.Dtos.Shippings;

public class CreateShippingOrderResponseDto
{
    public string AhamoveOrderId { get; set; }
    public string Status { get; set; }
    public decimal ShippingFeeActualCost { get; set; }
    public string Message { get; set; }
}

