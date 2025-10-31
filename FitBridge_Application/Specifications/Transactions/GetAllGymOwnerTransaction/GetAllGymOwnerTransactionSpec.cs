using System;

using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Transactions.GetAllGymOwnerTransaction;

public class GetAllGymOwnerTransactionSpec : BaseSpecification<Transaction>
{
    public GetAllGymOwnerTransactionSpec(GetAllGymOwnerTransactionParams parameters, Guid gymOwnerId) : base(x =>
    x.Status == TransactionStatus.Success &&
    x.IsEnabled &&
        (
            (x.TransactionType != TransactionType.DistributeProfit && x.OrderId != null && x.Order.OrderItems.Any(oi => oi.GymCourseId != null && oi.GymCourse!.GymOwnerId == gymOwnerId))
            ||
            (x.TransactionType == TransactionType.DistributeProfit && x.WalletId != null && x.Wallet.User.Id == gymOwnerId)
        )
    )
    {
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
        AddInclude(x => x.Order);
        AddInclude(x => x.Order.OrderItems);
        AddInclude("Order.OrderItems.GymCourse");
        AddInclude(x => x.Order.Coupon);
        AddInclude(x => x.PaymentMethod);
        AddInclude(x => x.Order.Account);
    }
}
