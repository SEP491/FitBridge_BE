using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerIdAndPtId;

public class GetCustomerPurchasedByCustomerIdAndPtIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByCustomerIdAndPtIdSpec(Guid customerId, Guid ptId) : base(x => x.CustomerId == customerId
    && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow)
    && x.OrderItems.Any(x => x.FreelancePTPackage != null && x.FreelancePTPackage.PtId == ptId))
    {
        AddInclude(x => x.OrderItems);
    }
}
