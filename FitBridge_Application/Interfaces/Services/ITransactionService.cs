using System;

namespace FitBridge_Application.Interfaces.Services;

public interface ITransactionService
{
    Task<bool> ExtendCourse(long orderCode);
    Task<bool> PurchasePt(long orderCode);
    Task<bool> DistributeProfit(Guid customerPurchasedId);
}
