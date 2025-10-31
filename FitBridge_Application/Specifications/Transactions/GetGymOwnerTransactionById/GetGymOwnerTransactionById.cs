using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Transactions.GetGymOwnerTransactionById;

public class GetGymOwnerTransactionByIdSpec : BaseSpecification<Transaction>
{
    public GetGymOwnerTransactionByIdSpec(Guid transactionId) : base(x =>
    x.Id == transactionId &&
    x.IsEnabled)
    {
        AddInclude(x => x.Order);
        AddInclude(x => x.Order.OrderItems);
        AddInclude("Order.OrderItems.GymCourse");
        AddInclude(x => x.Order.Coupon);
        AddInclude(x => x.PaymentMethod);
        AddInclude(x => x.Order.Account);
    }
}
