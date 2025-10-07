using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Transactions;

public class GetTransactionByOrderCodeSpec : BaseSpecification<Transaction>
{
    public GetTransactionByOrderCodeSpec(long orderCode, bool isInclude = true) : base(x => x.OrderCode == orderCode && x.TransactionType != TransactionType.DistributeProfit && x.IsEnabled == true)
    {
        if (isInclude)
        {
            AddInclude(x => x.Order);
        }

    }
}
