using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtId;

public class GetCustomerPurchasedByFreelancePtIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByFreelancePtIdSpec(Guid freelancePtId) : base(x => x.OrderItem.FreelancePTPackage.PtId == freelancePtId && x.IsEnabled && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow))
    {
    }
}
