using FitBridge_Application.Dtos.Orders;
using FitBridge_Application.Dtos.Shippings;

namespace FitBridge_Application.Interfaces.Services;

public interface IAhamoveService
{
    Task<AhamoveOrderResponseDto> CreateOrderAsync(AhamoveCreateOrderDto request);
    Task<string> GetTokenAsync();
    Task ProcessWebhookAsync(AhamoveWebhookDto webhookData);
    Task<bool> CancelShippingOrderAsync(Guid orderId, string comment);
    Task<ShippingEstimateDto> GetShippingPriceAsync(AhamovePriceEstimateDto request);
}

