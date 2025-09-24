using System;
using System.Text.Json;
using FitBridge_Application.Configurations;
using FitBridge_Application.Dtos.Payments;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Accounts;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;

namespace FitBridge_Infrastructure.Services;

public class PayOSService : IPayOSService
{
    private readonly PayOSSettings _settings;
    private readonly ILogger<PayOSService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PayOS _payOS;

    public PayOSService(
        IOptions<PayOSSettings> settings, 
        ILogger<PayOSService> logger,
        IUnitOfWork unitOfWork)
    {
        _settings = settings.Value;
        _logger = logger;
        _unitOfWork = unitOfWork;

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
                amount: (int)request.TotalAmount,
                description: user.UserName,
                items: items,
                cancelUrl: _settings.CancelUrl,
                returnUrl: _settings.ReturnUrl,
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

    // public async Task<PaymentInfoResponseDto> GetPaymentInfoAsync(string id)
    // {
    //     try
    //     {
    //         // Parse order code (id can be orderCode or paymentLinkId)
    //         if (!long.TryParse(id, out var orderCode))
    //         {
    //             throw new ArgumentException("Invalid order code format");
    //         }

    //         var paymentInfo = await _payOS.getPaymentLinkInformation(orderCode);

    //         if (paymentInfo == null)
    //         {
    //             throw new Exception($"Payment information not found for order {id}");
    //         }

    //         _logger.LogInformation("Successfully retrieved payment info for order {OrderCode}", orderCode);

    //         // Convert PayOS SDK result to our DTO format
    //         return new PaymentInfoResponseDto
    //         {
    //             Code = "00",
    //             Description = "Success",
    //             Data = new PaymentInfoDataDto
    //             {
    //                 Id = paymentInfo.id,
    //                 OrderCode = (int)paymentInfo.orderCode,
    //                 Amount = paymentInfo.amount,
    //                 AmountPaid = paymentInfo.amountPaid,
    //                 AmountRemaining = paymentInfo.amountRemaining,
    //                 Status = paymentInfo.status,
    //                 CreatedAt = DateTime.Parse(paymentInfo.createdAt),
    //                 CancellationReason = paymentInfo.cancellationReason,
    //                 CanceledAt = paymentInfo.canceledAt != null ? DateTime.Parse(paymentInfo.canceledAt) : null,
    //                 Transactions = paymentInfo.transactions.Select(t => new TransactionDto
    //                 {
    //                     Reference = t.reference,
    //                     Amount = t.amount,
    //                     AccountNumber = t.accountNumber,
    //                     Description = t.description,
    //                     TransactionDateTime = DateTime.Parse(t.transactionDateTime),
    //                     VirtualAccountName = t.virtualAccountName,
    //                     VirtualAccountNumber = t.virtualAccountNumber,
    //                     CounterAccountBankId = t.counterAccountBankId,
    //                     CounterAccountBankName = t.counterAccountBankName,
    //                     CounterAccountName = t.counterAccountName,
    //                     CounterAccountNumber = t.counterAccountNumber
    //                 }).ToList()
    //             }
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting payment info for {Id}", id);
    //         throw;
    //     }
    // }

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

    // public async Task<bool> HandlePaymentWebhookAsync(string webhookData)
    // {
    //     try
    //     {
    //         // Parse webhook data
    //         var webhookType = JsonSerializer.Deserialize<WebhookType>(webhookData, new JsonSerializerOptions
    //         {
    //             PropertyNameCaseInsensitive = true
    //         });

    //         if (webhookType == null)
    //         {
    //             _logger.LogWarning("Invalid webhook payload received");
    //             return false;
    //         }

    //         // Verify webhook data using PayOS SDK
    //         var verifiedWebhookData = _payOS.verifyPaymentWebhookData(webhookType);

    //         if (verifiedWebhookData == null)
    //         {
    //             _logger.LogWarning("Failed to verify webhook data");
    //             return false;
    //         }

    //         // Find transaction by order code
    //         var transactions = await _unitOfWork.Repository<Transaction>()
    //             .GetAllWithSpecificationAsync(new TransactionByReferenceCodeSpecification(verifiedWebhookData.orderCode));

    //         var transaction = transactions.FirstOrDefault();
    //         if (transaction == null)
    //         {
    //             _logger.LogWarning("Transaction not found for order code {OrderCode}", verifiedWebhookData.orderCode);
    //             return false;
    //         }

    //         var paymentStatus = verifiedWebhookData.code switch
    //         {
    //             "00" => TransactionStatus.Completed, // PayOS success code
    //             "01" => TransactionStatus.Failed,    // PayOS failure code
    //             _ => TransactionStatus.Pending
    //         };
            
    //         // Update transaction status based on webhook data
    //         transaction.TransactionStatus = verifiedWebhookData.code switch
    //         {
    //             "00" => TransactionStatus.Completed, // PayOS success code
    //             "01" => TransactionStatus.Failed,    // PayOS failure code
    //             _ => TransactionStatus.Pending
    //         };

    //         _unitOfWork.Repository<TransactionRecord>().Update(transaction);
    //         await _unitOfWork.CompleteAsync();

    //         // If payment is successful and it's a membership payment, activate the membership
    //         if (transaction.TransactionStatus == TransactionStatus.Completed && transaction.MembershipId.HasValue)
    //         {
    //             await _membershipsService.ProcessMembershipPaymentSuccessAsync((int)verifiedWebhookData.orderCode);
    //         }

    //         _logger.LogInformation("Successfully processed webhook for order code {OrderCode}, status: {Status}", 
    //             verifiedWebhookData.orderCode, verifiedWebhookData.desc);

    //         return true;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error processing payment webhook");
    //         return false;
    //     }
    // }

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
