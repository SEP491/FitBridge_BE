using System;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;

public class GetCustomerPurchasedByCustomerIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByCustomerIdSpec(Guid accountId) : base(x => x.CustomerId == accountId)
    {
        AddInclude(x => x.OrderItems);
        AddInclude("OrderItems.GymCourse");
        AddInclude("OrderItems.GymCourse.GymCoursePts");
    }
}
