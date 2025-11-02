using System;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Transactions.GetAllTransactionAdmin;

public class GetAllTransactionAdminSpec : BaseSpecification<Transaction>
{
    public GetAllTransactionAdminSpec(GetAllTransactionAdminParams parameters) : base(x => x.IsEnabled)
    {
        AddInclude(x => x.Order);
        AddInclude(x => x.Order.Account);
        AddInclude(x => x.Order.OrderItems);
        AddInclude(x => x.PaymentMethod);
        if(parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        } else {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
