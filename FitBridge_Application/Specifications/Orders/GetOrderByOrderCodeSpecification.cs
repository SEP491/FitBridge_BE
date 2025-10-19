using System;
using FitBridge_Application.Interfaces.Specifications;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Orders;

public class GetOrderByOrderCodeSpecification : BaseSpecification<Order>
{
    public GetOrderByOrderCodeSpecification(long orderCode) : base(o => o.Transactions.Any(t => t.OrderCode == orderCode))
    {
        AddInclude(o => o.Transactions);
        AddInclude(o => o.OrderItems);
        AddInclude("OrderItems.FreelancePTPackage");
        AddInclude("OrderItems.GymCourse");
        AddInclude("OrderItems.ServiceInformation");
        AddInclude("OrderItems.ProductDetail");
        AddInclude("OrderItems.GymCourse");
        AddInclude(o => o.Coupon);
        AddInclude(o => o.Coupon.Creator);
    }
}
