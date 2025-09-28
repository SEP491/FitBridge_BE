using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Transactions;

public class GetTransactionByOrderCodeWithIncludeSpec : BaseSpecification<Transaction>
{
    public GetTransactionByOrderCodeWithIncludeSpec(long orderCode) : base(x => x.OrderCode == orderCode && x.IsEnabled == true)
    {
        AddInclude(x => x.Order);
        AddInclude(x => x.Order.OrderItems);
        AddInclude(x => x.Order.CustomerPurchasedToExtend);
        AddInclude(x => x.Order.CustomerPurchasedToExtend.OrderItems);
        AddInclude("Order.OrderItems.FreelancePTPackage");
        AddInclude("Order.OrderItems.GymCourse");
        AddInclude("Order.OrderItems.ServiceInformation");
        AddInclude("Order.OrderItems.ProductDetail");
        AddInclude("Order.OrderItems.GymCourse");
        AddInclude("Order.GymCoursePTToAssign");
    }
}
