using FitBridge_Application.Dtos.Shippings;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Orders;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services;

public class AhamoveWebhookService : IAhamoveWebhookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AhamoveWebhookService> _logger;

    public AhamoveWebhookService(
        IUnitOfWork unitOfWork,
        ILogger<AhamoveWebhookService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ProcessWebhookAsync(AhamoveWebhookDto webhookData)
    {
        try
        {
            _logger.LogInformation($"Processing webhook for Ahamove Order ID: {webhookData.Id}, Status: {webhookData.Status}, SubStatus: {webhookData.SubStatus}");

            // Find order by Ahamove order ID
            var order = await _unitOfWork.Repository<Order>().GetBySpecificationAsync(
                new GetOrderByAhamoveOrderIdSpecification(webhookData.Id), false);

            if (order == null)
            {
                _logger.LogWarning($"Order not found for Ahamove Order ID: {webhookData.Id}");
                return;
            }

            // Determine the new status based on Ahamove webhook data
            var (newStatus, statusDescription) = DetermineOrderStatus(webhookData);

            // Only update if status has changed
            if (order.Status != newStatus)
            {
                var oldStatus = order.Status;
                order.Status = newStatus;

                // Create order status history entry
                var statusHistory = new OrderStatusHistory
                {
                    OrderId = order.Id,
                    Status = newStatus,
                    Description = statusDescription
                };

                _unitOfWork.Repository<OrderStatusHistory>().Insert(statusHistory);
                _unitOfWork.Repository<Order>().Update(order);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation($"Order {order.Id} status updated from {oldStatus} to {newStatus}. Description: {statusDescription}");
            }
            else
            {
                _logger.LogInformation($"Order {order.Id} status unchanged: {newStatus}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing webhook for Ahamove Order ID: {webhookData.Id}");
            throw;
        }
    }

    private (OrderStatus status, string description) DetermineOrderStatus(AhamoveWebhookDto webhookData)
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
                if (subStatus == "ARRIVED")
                {
                    // Check if delivery failed at destination
                    var deliveryPath = webhookData.Path?.Skip(1).FirstOrDefault(); // Second path is delivery address
                    if (deliveryPath?.Status == "FAILED")
                    {
                        return (OrderStatus.Shipping, $"Giao hàng thất bại: {deliveryPath.FailComment}. Đang xử lý.");
                    }
                    return (OrderStatus.Arrived, "Tài xế đã đến nơi giao hàng");
                }
                return (OrderStatus.Shipping, "Đang giao hàng");

            case "COMPLETED":
                if (subStatus == "IN_RETURN")
                {
                    // Package is being returned to sender
                    return (OrderStatus.Shipping, "Đang hoàn trả hàng về người gửi");
                }
                else if (subStatus == "RETURNED")
                {
                    // Package was returned to sender - customer did not receive
                    var deliveryPath = webhookData.Path?.Skip(1).FirstOrDefault();
                    var failReason = deliveryPath?.FailComment ?? "Không xác định";
                    return (OrderStatus.CustomerNotReceived, $"Đã hoàn trả hàng. Lý do: {failReason}");
                }
                else
                {
                    // Successfully delivered
                    var deliveryPath = webhookData.Path?.Skip(1).FirstOrDefault();
                    if (deliveryPath?.Status == "COMPLETED")
                    {
                        return (OrderStatus.Finished, "Giao hàng thành công");
                    }
                    // If path status is FAILED but main status is COMPLETED, it might be returned
                    if (deliveryPath?.Status == "FAILED")
                    {
                        return (OrderStatus.CustomerNotReceived, $"Giao hàng thất bại: {deliveryPath.FailComment}");
                    }
                    return (OrderStatus.Finished, "Hoàn thành đơn hàng");
                }

            case "CANCELLED":
                // Order was cancelled
                var cancelDescription = "Đơn hàng đã bị hủy";
                if (webhookData.CancelByUser == true)
                {
                    cancelDescription = $"Đơn hàng bị hủy bởi người dùng: {webhookData.CancelComment}";
                }
                else if (!string.IsNullOrEmpty(webhookData.CancelComment))
                {
                    cancelDescription = $"Đơn hàng bị hủy: {webhookData.CancelComment}";
                }
                return (OrderStatus.Cancelled, cancelDescription);

            default:
                _logger.LogWarning($"Unknown Ahamove status: {status}, SubStatus: {subStatus}");
                return (OrderStatus.Processing, $"Trạng thái không xác định: {status}");
        }
    }
}
