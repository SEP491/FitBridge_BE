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
using FitBridge_Application.Commons.Constants;
using Quartz;
using FitBridge_Infrastructure.Jobs;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace FitBridge_Infrastructure.Services;

public class PayOSService : IPayOSService
{
    private readonly PayOSSettings _settings;

    private readonly ILogger<PayOSService> _logger;

    private readonly IUnitOfWork _unitOfWork;

    private readonly PayOS _payOS;

    private readonly ITransactionService _transactionService;

    private readonly ISchedulerFactory _schedulerFactory;

    public PayOSService(
        IOptions<PayOSSettings> settings,
        ILogger<PayOSService> logger,
        IUnitOfWork unitOfWork,
        ITransactionService transactionService,
        ISchedulerFactory schedulerFactory)
    {
        _settings = settings.Value;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _transactionService = transactionService;
        // Initialize PayOS SDK
        _payOS = new PayOS(_settings.ClientId, _settings.ApiKey, _settings.ChecksumKey);
        _schedulerFactory = schedulerFactory;
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
                description: user.PhoneNumber,
                items: items,
                cancelUrl: $"{_settings.CancelUrl}?code=01&message&orderCode={orderCode}&amount={request.TotalAmountPrice}",
                returnUrl: $"{_settings.ReturnUrl}?code=00&message&orderCode={orderCode}&amount={request.TotalAmountPrice}",
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

            _logger.LogInformation($"Successfully retrieved payment info for order {orderCode}", orderCode);

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
            if (verifiedWebhookData.orderCode == 123)
            {
                return true; // Test webhook from PayOS
            }

            var transaction = await _unitOfWork.Repository<FitBridge_Domain.Entities.Orders.Transaction>().GetBySpecificationAsync(new GetTransactionByOrderCodeSpec(verifiedWebhookData.orderCode));

            if (transaction == null)
            {
                throw new NotFoundException("Transaction not found");
            }
            if (transaction.Status == TransactionStatus.Success)
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
            
            if (transaction.TransactionType == TransactionType.AssignPt)
            {
                return await _transactionService.PurchasePt(verifiedWebhookData.orderCode);
            }
            
            if (transaction.TransactionType == TransactionType.FreelancePTPackage)
            {
                return await _transactionService.PurchaseFreelancePTPackage(verifiedWebhookData.orderCode);
            }
            
            if (transaction.TransactionType == TransactionType.GymCourse)
            {
                return await _transactionService.PurchaseGymCourse(verifiedWebhookData.orderCode);
            }

            if(transaction.TransactionType == TransactionType.ExtendFreelancePTPackage)
            {
                return await _transactionService.ExtendFreelancePTPackage(verifiedWebhookData.orderCode);
            }

            if(transaction.TransactionType == TransactionType.SubscriptionPlansOrder)
            {
                return await _transactionService.PurchaseSubscriptionPlans(verifiedWebhookData.orderCode);
            }

            return false;
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
}