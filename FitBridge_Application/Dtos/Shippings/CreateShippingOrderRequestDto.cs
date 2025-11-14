namespace FitBridge_Application.Dtos.Shippings;

public class CreateShippingOrderRequestDto
{
    public Guid OrderId { get; set; }
    public AhamovePathDto PickupAddress { get; set; }
    public AhamovePathDto DeliveryAddress { get; set; }
    public string? Remarks { get; set; }
}

