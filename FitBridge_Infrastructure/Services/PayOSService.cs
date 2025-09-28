using System;
using System.Text.Json;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Orders;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Exceptions;
using FitBridge_Domain.Enums.Orders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Application.Specifications.GymCoursePts.GetGymCoursePtById;
using FitBridge_Application.Specifications.Transactions;
using FitBridge_Domain.Enums.Payments;

namespace FitBridge_Infrastructure.Services;

public class PayOSService : IPayOSService
{
    private readonly PayOSSettings _settings;

    private readonly ILogger<PayOSService> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly PayOS _payOS;

    private readonly ITransactionService _transactionService;

    public PayOSService(
        IOptions<PayOSSettings> settings,
        ILogger<PayOSService> logger,
        IUnitOfWork unitOfWork,
        ITransactionService transactionService)
    {
        _settings = settings.Value;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _transactionService = transactionService;
        // Initialize PayOS SDK
        _payOS = new PayOS(_settings.ClientId, _settings.ApiKey, _settings.ChecksumKey);
    }

    public async Task<PaymentResponseDto> CreatePaymentLinkAsync(CreatePaymentRequestDto request, ApplicationUser user)
    {
        try
        {
            // Convert request items to PayOS SDK format
            var items = request.OrderItems.Select(i => new ItemData(i.ProductName, i.Quantity, (int)i.Price)).ToList();
            var orderCode = GenerateOrderCode();
            var address = "";
            if (request.AddressId.HasValue)
            {
                var addressEntity = await _unitOfWork.Repository<Address>().GetByIdAsync(request.AddressId.Value);
                if (addressEntity == null)
                {
                    throw new NotFoundException("Address not found");
                }
                address = $"{addressEntity.Street}, {addressEntity.Ward}, {addressEntity.District}, {addressEntity.City}";
            }
            var paymentData = new PaymentData(
                orderCode: orderCode,
                //amount: (int)request.TotalAmount,
                amount: 5000,
                description: user.UserName,
                items: items,
                cancelUrl: $"{_settings.CancelUrl}?code=01&message&orderCode={orderCode}&amount={request.TotalAmount}",
                returnUrl: $"{_settings.ReturnUrl}?code=00&message&orderCode={orderCode}&amount={request.TotalAmount}",
                expiredAt: DateTimeOffset.UtcNow.AddMinutes(_settings.ExpirationMinutes).ToUnixTimeSeconds(),
                buyerName: user.UserName,
                buyerEmail: user.Email,
                buyerPhone: user.PhoneNumber,
                buyerAddress: address
            );

            var createPaymentResult = await _payOS.createPaymentLink(paymentData);

            // Convert PayOS SDK result to our DTO format
            return new PaymentResponseDto
            {
                Code = "00", // Success code
                Description = "Success",
                Data = createPaymentResult != null ? new PaymentDataDto
                {
                    PaymentLinkId = createPaymentResult.paymentLinkId,
                    CheckoutUrl = createPaymentResult.checkoutUrl,
                    QrCode = createPaymentResult.qrCode,
                    OrderCode = createPaymentResult.orderCode,
                    Amount = createPaymentResult.amount,
                    Status = createPaymentResult.status,
                    Currency = "VND"
                } : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment link");
            throw;
        }
    }

    public async Task<PaymentInfoResponseDto> GetPaymentInfoAsync(string id)
    {
        try
        {
            // Parse order code (id can be orderCode or paymentLinkId)
            if (!long.TryParse(id, out var orderCode))
            {
                throw new ArgumentException("Invalid order code format");
            }

            var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

            if (paymentInfo == null)
            {
                throw new Exception($"Payment information not found for order {id}");
            }

            _logger.LogInformation("Successfully retrieved payment info for order {OrderCode}", orderCode);

            // Convert PayOS SDK result to our DTO format
            return new PaymentInfoResponseDto
            {
                Code = "00",
                Description = "Success",
                Data = new PaymentInfoDataDto
                {
                    Id = paymentInfo.id,
                    OrderCode = (int)paymentInfo.orderCode,
                    Amount = paymentInfo.amount,
                    AmountPaid = paymentInfo.amountPaid,
                    AmountRemaining = paymentInfo.amountRemaining,
                    Status = paymentInfo.status,
                    CreatedAt = DateTime.Parse(paymentInfo.createdAt),
                    CancellationReason = paymentInfo.cancellationReason,
                    CanceledAt = paymentInfo.canceledAt != null ? DateTime.Parse(paymentInfo.canceledAt) : null,
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment info for {Id}", id);
            throw;
        }
    }

    public async Task<bool> CancelPaymentAsync(string id, string? cancellationReason = null)
    {
        try
        {
            // Parse order code
            if (!long.TryParse(id, out var orderCode))
            {
                _logger.LogError("Invalid order code format: {Id}", id);
                return false;
            }

            var cancelledPayment = await _payOS.cancelPaymentLink(orderCode, cancellationReason ?? "User cancelled");

            if (cancelledPayment != null)
            {
                _logger.LogInformation("Successfully cancelled payment {Id}", id);
                return true;
            }

            _logger.LogError("Failed to cancel payment {Id}", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling payment {Id}", id);
            return false;
        }
    }

    public async Task<bool> HandlePaymentWebhookAsync(string webhookData)
    {
        try
        {
            // Parse webhook data
            var webhookType = JsonSerializer.Deserialize<WebhookType>(webhookData, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (webhookType == null)
            {
                _logger.LogWarning("Invalid webhook payload received");
                return false;
            }

            // Verify webhook data using PayOS SDK
            var verifiedWebhookData = _payOS.verifyPaymentWebhookData(webhookType);

            if (verifiedWebhookData == null)
            {
                _logger.LogWarning("Failed to verify webhook data");
                return false;
            }
            if(verifiedWebhookData.orderCode == 123)
            {
                 return true; // Test webhook from PayOS
            }
            
            var transaction = await _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeSpec(verifiedWebhookData.orderCode));

            if (transaction == null)
            {
                throw new NotFoundException("Transaction not found");
            }
            if(transaction.Status == TransactionStatus.Success)
            {
                return true; // Already processed, prevent from duplicate processing of webhook
            }

            transaction.Status = TransactionStatus.Success;
            _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().Update(transaction);
            await _unitOfWork.CommitAsync();

            if (transaction.TransactionType == TransactionType.ExtendCourse)
            {
                return await _transactionService.ExtendCourse(verifiedWebhookData.orderCode);
            }
            if(transaction.TransactionType == TransactionType.AssignPt)
            {
                return await _transactionService.PurchasePt(verifiedWebhookData.orderCode);
            }

            // Find transaction by order code
            var OrderEntity = await _unitOfWork.Repository<Order>()
                .GetBySpecificationAsync(new GetOrderByOrderCodeSpecification(verifiedWebhookData.orderCode), false);
            if (OrderEntity == null)
            {
                throw new NotFoundException("Order not found");
            }
            if (OrderEntity.OrderItems.Any(item => item.ProductDetailId != null))
            {
                OrderEntity.Status = OrderStatus.Pending;
            }
            else
            {
                OrderEntity.Status = OrderStatus.Arrived;
            }
            foreach (var orderItem in OrderEntity.OrderItems)
            {
                if (orderItem.ProductDetailId == null)
                {
                    var numOfSession = 0;
                    if (orderItem.FreelancePTPackageId != null)
                    {
                        numOfSession = orderItem.FreelancePTPackage.NumOfSessions;
                    }
                    if (orderItem.GymCourseId != null && orderItem.GymPtId != null)
                    {
                        var gymCoursePT = await _unitOfWork.Repository<GymCoursePT>().GetBySpecificationAsync(new GetGymCoursePtByGymCourseIdAndPtIdSpec(orderItem.GymCourseId.Value, orderItem.GymPtId.Value));
                        if (gymCoursePT == null)
                        {
                            throw new NotFoundException("Gym course PT with gym course id and pt id not found");
                        }
                        numOfSession = gymCoursePT.Session.Value;
                    }
                    orderItem.CustomerPurchased = new CustomerPurchased
                    {
                        CustomerId = OrderEntity.AccountId,
                        AvailableSessions = orderItem.Quantity * numOfSession,
                        ExpirationDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(30 * orderItem.Quantity),
                    };
                }
            }
            await _unitOfWork.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment webhook");
            return false;
        }
    }

    private long GenerateOrderCode()
    {
        // Generate a unique order code using timestamp and random number
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random().Next(1000, 9999);
        // Ensure the result is positive and fits within long range
        var orderCode = (timestamp % 100000) * 10000 + random;
        return Math.Abs(orderCode);
    }

    // public async Task<bool> HandlePaymentAsync(string status, long orderCode)
    // {
    //     var transaction = await _unitOfWork.Repository<TransactionRecord>().GetBySpecificationAsync(new TransactionByReferenceCodeSpecification(orderCode));
    //     if (transaction == null)
    //     {
    //         _logger.LogWarning("Transaction not found for order code {OrderCode}", orderCode);
    //         return false;
    //     }
    //     transaction.TransactionStatus = status switch
    //     {
    //         "PAID" => TransactionStatus.Completed,
    //         "FAILED" => TransactionStatus.Failed,
    //         "CANCELLED" => TransactionStatus.Cancelled,
    //         _ => transaction.TransactionStatus
    //     };

    //     _unitOfWork.Repository<TransactionRecord>().Update(transaction);
    //         await _unitOfWork.CompleteAsync();

    //     if (transaction.TransactionStatus == TransactionStatus.Completed && transaction.MembershipId.HasValue)
    //     {
    //         await _membershipsService.ProcessMembershipPaymentSuccessAsync(transaction.ReferenceCode);
    //     }

    //     return true;
    // }
}