using FitBridge_Application.Dtos.Shippings;
using MediatR;

namespace FitBridge_Application.Features.Orders.CreateShippingOrder;

public class CreateShippingOrderCommand : IRequest<CreateShippingOrderResponseDto>
{
    public Guid OrderId { get; set; }
    public AhamovePathDto PickupAddress { get; set; }
    public AhamovePathDto DeliveryAddress { get; set; }
    public string? Remarks { get; set; }
}

