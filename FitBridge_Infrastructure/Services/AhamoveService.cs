using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Application.Specifications.Orders;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FitBridge_Application.Dtos.Orders;
using FitBridge_Domain.Entities.Accounts;
using Azure.Core;
using System.Text.Json.Serialization;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Infrastructure.Services;

public class AhamoveService : IAhamoveService
{
    private readonly HttpClient _httpClient;
    private readonly AhamoveSettings _ahamoveSettings;
    private readonly ILogger<AhamoveService> _logger;
    private string? _cachedToken;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScheduleJobServices _scheduleJobServices;
    private readonly SystemConfigurationService _systemConfigurationService;
    public AhamoveService(
        HttpClient httpClient,
        IOptions<AhamoveSettings> ahamoveSettings,
        ILogger<AhamoveService> logger,
        IUnitOfWork unitOfWork,
        IScheduleJobServices scheduleJobServices,
        SystemConfigurationService systemConfigurationService)
    {
        _httpClient = httpClient;
        _ahamoveSettings = ahamoveSettings.Value;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _httpClient.BaseAddress = new Uri(_ahamoveSettings.BaseUrl);
        _scheduleJobServices = scheduleJobServices;
        _systemConfigurationService = systemConfigurationService;
    }

    public async Task<string> GetTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            return _cachedToken;
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/v3/accounts/token");
            request.Headers.Add("cache-control", "no-cache");

            var payload = new
            {
                mobile = _ahamoveSettings.MobileNumber,
                api_key = _ahamoveSettings.ApiKey
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get Ahamove token. Status: {response.StatusCode}, Response: {responseContent}");
                throw new BusinessException($"Failed to authenticate with Ahamove: {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            _cachedToken = tokenResponse.GetProperty("token").GetString();

            return _cachedToken!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Ahamove token");
            throw;
        }
    }

    public async Task<AhamoveOrderResponseDto> CreateOrderAsync(AhamoveCreateOrderDto request)
    {
        try
        {
            var token = await GetTokenAsync();

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v3/orders");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Headers.Add("cache-control", "no-cache");

            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation($"Creating Ahamove order with payload: {jsonContent}");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to create Ahamove order. Status: {response.StatusCode}, Response: {responseContent}");
                throw new BusinessException($"Failed to create Ahamove order: {responseContent}");
            }

            _logger.LogInformation($"Ahamove order created successfully: {responseContent}");

            var orderResponse = JsonSerializer.Deserialize<AhamoveOrderResponseDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (orderResponse == null)
            {
                throw new BusinessException("Failed to deserialize Ahamove order response");
            }

            return orderResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Ahamove order");
            throw;
        }
    }

    public async Task ProcessWebhookAsync(AhamoveWebhookDto webhookData)
    {
        try
        {
            _logger.LogInformation($"Processing webhook for Ahamove Order ID: {webhookData.Id}, Status: {webhookData.Status}, SubStatus: {webhookData.SubStatus}");

            // Find order by Ahamove order ID
            var order = await _unitOfWork.Repository<Order>().GetBySpecificationAsync(new GetOrderByAhamoveOrderIdSpecification(webhookData.Id), false);

            if (order == null)
            {
                _logger.LogWarning($"Order not found for Ahamove Order ID: {webhookData.Id}");
                return;
            }

            // Determine the new status based on Ahamove webhook data
            var (newStatus, statusDescription) = DetermineOrderStatus(webhookData, order);
            var oldStatus = order.Status;

            if (newStatus == OrderStatus.Arrived)
            {
                int autoFinishArrivedOrderAfterTime = (int)await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.AutoFinishArrivedOrderAfterTime);
                var transactionToUpdate = order.Transactions.FirstOrDefault(t => t.TransactionType == TransactionType.ProductOrder)!;
                if (transactionToUpdate.PaymentMethod.MethodType == MethodType.COD)
                {
                    transactionToUpdate.Status = TransactionStatus.Success;
                }

                await _scheduleJobServices.ScheduleAutoFinishArrivedOrderJob(order.Id, DateTime.UtcNow.AddDays(autoFinishArrivedOrderAfterTime));

                var autoMarkAsFeedbackAfterDays = (int)await _systemConfigurationService.GetSystemConfigurationAutoConvertDataTypeAsync(ProjectConstant.SystemConfigurationKeys.AutoMarkAsFeedbackAfterDays);
                foreach(var orderItem in order.OrderItems)
                {
                    await _scheduleJobServices.ScheduleAutoMarkAsFeedbackJob(orderItem.Id, DateTime.UtcNow.AddDays(autoMarkAsFeedbackAfterDays));
                }
            }
            if (newStatus == OrderStatus.Returned)
            {
                var paymentMethod = await _unitOfWork.Repository<PaymentMethod>().GetByIdAsync(order.Transactions.FirstOrDefault(t => t.TransactionType == TransactionType.ProductOrder)!.PaymentMethodId);
                if (paymentMethod == null)
                {
                    throw new BusinessException("Payment method not found");
                }
                if (paymentMethod.MethodType == MethodType.COD)
                {
                    var returnStatusHistory = new OrderStatusHistory
                    {
                        OrderId = order.Id,
                        Status = OrderStatus.Returned,
                        Description = $"Đã hoàn trả hàng. Lý do: {webhookData.CancelComment}",
                        PreviousStatus = OrderStatus.InReturn,
                    };
                    foreach (var orderItem in order.OrderItems)
                    {
                        orderItem.ProductDetail.Quantity += orderItem.Quantity;
                        orderItem.ProductDetail.SoldQuantity -= orderItem.Quantity;
                    }
                    oldStatus = OrderStatus.Returned;
                    newStatus = OrderStatus.Cancelled;
                    _unitOfWork.Repository<OrderStatusHistory>().Insert(returnStatusHistory);
                }
            }

            if (newStatus == OrderStatus.Shipping)
            {
                if (oldStatus == OrderStatus.Accepted)
                {
                    order.ShippingFeeActualCost += webhookData.TotalPay;
                    var shippingFeeDifference = order.ShippingFeeActualCost - order.ShippingFee;
                    order.Transactions.FirstOrDefault(t => t.TransactionType == TransactionType.ProductOrder)!.ProfitAmount -= shippingFeeDifference;
                }
            }
 
            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            var statusHistoryToUpdate = order.OrderStatusHistories.Where(s => s.Status == oldStatus).OrderByDescending(s => s.CreatedAt).FirstOrDefault();
            if (statusHistoryToUpdate != null && oldStatus != newStatus)
            {
                statusHistoryToUpdate.Description = statusDescription;
            }

            // Only insert status history if status has changed
            if (oldStatus != newStatus)
            {
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = newStatus,
                    Description = statusDescription,
                    PreviousStatus = oldStatus,
                };
                _unitOfWork.Repository<OrderStatusHistory>().Insert(statusHistory);
            }

            await _unitOfWork.CommitAsync();

            _logger.LogInformation($"Order {order.Id} status updated from {oldStatus} to {newStatus}. Description: {statusDescription}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing webhook for Ahamove Order ID: {webhookData.Id}");
            throw new BusinessException($"Error processing webhook for Ahamove Order ID: {webhookData.Id}: {ex.Message}");
        }
    }

    private (OrderStatus status, string description) DetermineOrderStatus(AhamoveWebhookDto webhookData, Order order)
    {
        var status = webhookData.Status.ToUpper();
        var subStatus = webhookData.SubStatus?.ToUpper();

        switch (status)
        {
            case "ASSIGNING":
                // Driver is being assigned or looking for a driver
                var description = "Đang tìm tài xế";
                if (!string.IsNullOrEmpty(webhookData.RebroadcastComment))
                {
                    description += $" - {webhookData.RebroadcastComment}";
                }
                return (OrderStatus.Assigning, description);

            case "ACCEPTED":
                // Driver accepted the order
                return (OrderStatus.Accepted, $"Tài xế {webhookData.SupplierName} đã nhận đơn");

            case "IN PROCESS":
                // Order is being delivered
                if (subStatus != null && subStatus == "ARRIVED")
                {
                    // Check if delivery failed at destination
                    var deliveryPath = webhookData.Path?.Skip(1).FirstOrDefault(); // Second path is delivery address
                    if (deliveryPath?.Status == "FAILED")
                    {
                        return (OrderStatus.Shipping, $"Giao hàng thất bại: {deliveryPath.FailComment}. Đang xử lý.");
                    }
                    throw new BusinessException("Trạng thái không xác định");
                }
                return (OrderStatus.Shipping, "Đang giao hàng");

            case "COMPLETED":
                if (subStatus == "IN_RETURN")
                {
                    // Package is being returned to sender
                    return (OrderStatus.InReturn, "Đang hoàn trả hàng về người gửi");
                }
                else if (subStatus == "RETURNED")
                {

                    // Package was returned to sender - customer did not receive
                    var deliveryPath = webhookData.Path?.Skip(1).FirstOrDefault();
                    var failReason = deliveryPath?.FailComment ?? "Không xác định";
                    return (OrderStatus.Returned, $"Đã hoàn trả hàng. Lý do: {failReason}");
                }
                else
                {
                    // Successfully delivered
                    var deliveryPath = webhookData.Path?.Skip(1).FirstOrDefault();
                    if (deliveryPath?.Status == "COMPLETED")
                    {
                        return (OrderStatus.Arrived, "Giao hàng thành công");
                    }
                    throw new BusinessException("Trạng thái không xác định");
                }

            case "CANCELLED":
                // Order was cancelled
                var cancelDescription = "Đơn hàng đã bị hủy";
                if (webhookData.CancelByUser == true)
                {
                    cancelDescription = $"Đơn hàng bị hủy bởi admin: {webhookData.CancelComment}";
                }
                else if (!string.IsNullOrEmpty(webhookData.CancelComment))
                {
                    cancelDescription = $"Đơn hàng bị hủy bởi tài xế: {webhookData.CancelComment}";
                }
                return (OrderStatus.Processing, cancelDescription);

            default:
                _logger.LogWarning($"Unknown Ahamove status: {status}, SubStatus: {subStatus}");
                return (OrderStatus.Processing, $"Trạng thái không xác định: {status}");
        }
    }

    public async Task<bool> CancelShippingOrderAsync(Guid orderId, string comment)
    {
        try
        {
            var token = await GetTokenAsync();
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/v3/orders/tracks");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Headers.Add("cache-control", "no-cache");

            var jsonContent = JsonSerializer.Serialize(new { tracking_number = orderId, comment = comment });
            httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to cancel Ahamove shipping order. Status: {response.StatusCode}, Response: {responseContent}");
                throw new BusinessException($"Failed to cancel Ahamove shipping order: {responseContent}");
            }

            _logger.LogInformation($"Ahamove shipping order canceled successfully: {responseContent}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling Ahamove shipping order");
            throw new BusinessException($"Failed to cancel Ahamove shipping order: {ex.Message}");
        }
    }

    public async Task<ShippingEstimateDto> GetShippingPriceAsync(AhamovePriceEstimateDto request)
    {
        try
        {
            var token = await GetTokenAsync();
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/v3/orders/estimates");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Headers.Add("cache-control", "no-cache");
            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
            httpRequest.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get Ahamove shipping price. Status: {response.StatusCode}, Response: {responseContent}");
                throw new BusinessException($"Failed to get Ahamove shipping price: {responseContent}");
            }
            var responseDtos = JsonSerializer.Deserialize<List<ShippingEstimateWrapper>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });
            var responseDto = responseDtos?.FirstOrDefault();
            if (responseDto == null)
            {
                throw new BusinessException("Failed to deserialize Ahamove price estimate response");
            }
            return responseDto.data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Ahamove shipping price");
            throw new BusinessException($"Failed to get Ahamove shipping price: {ex.Message}");
        }
    }
}

