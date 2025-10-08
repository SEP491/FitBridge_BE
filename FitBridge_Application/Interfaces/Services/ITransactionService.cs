using System;
using FitBridge_Application.Dtos.Jobs;

namespace FitBridge_Application.Interfaces.Services;

public interface ITransactionService
{
    Task<bool> ExtendCourse(long orderCode);
    Task<bool> PurchasePt(long orderCode);
    Task<bool> DistributeProfit(Guid customerPurchasedId);
    Task<bool> PurchaseFreelancePTPackage(long orderCode);
    Task<bool> PurchaseGymCourse(long orderCode);
}
