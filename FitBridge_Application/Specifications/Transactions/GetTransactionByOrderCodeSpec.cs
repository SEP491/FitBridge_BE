using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Transactions;

public class GetTransactionByOrderCodeSpec : BaseSpecification<Transaction>
{
    public GetTransactionByOrderCodeSpec(long orderCode, bool isInclude = false) : base(x => x.OrderCode == orderCode && x.IsEnabled == true)
    {
        AddInclude(x => x.Order);
    }
}
