using FitBridge_Application.Dtos.Shippings;

namespace FitBridge_Application.Interfaces.Services;

public interface IAhamoveService
{
    Task<AhamoveOrderResponseDto> CreateOrderAsync(AhamoveCreateOrderDto request);
    Task<string> GetTokenAsync();
}

