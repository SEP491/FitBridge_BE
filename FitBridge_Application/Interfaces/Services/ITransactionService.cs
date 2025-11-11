using System;
using FitBridge_Application.Dtos.Jobs;
using FitBridge_Application.Dtos.Payments.ApplePaymentDto;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Interfaces.Services;

public interface ITransactionService
{
    Task<bool> ExtendCourse(long orderCode);
    Task<bool> PurchasePt(long orderCode);
    Task<bool> DistributeProfit(Guid orderItemId);
    Task<bool> PurchaseFreelancePTPackage(long orderCode);
    Task<bool> PurchaseGymCourse(long orderCode);
    Task<bool> ExtendFreelancePTPackage(long orderCode);
    Task<bool> DistributePendingProfit(Guid CustomerPurchasedId);
    Task<decimal> CalculateMerchantProfit(OrderItem orderItem, Coupon? coupon = null);
    Task<decimal> CalculateSystemProfit(Order order);
    Task<bool> PurchaseSubscriptionPlans(long orderCode);
    Task<bool> PurchaseAppleSubscriptionPlans(AsnDecodedPayload asnDecodedPayload, JwsTransactionDecoded jwsTransactionDecoded);
}
