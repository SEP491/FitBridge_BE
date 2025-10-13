using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByFreelancePtIdCount;

public class GetCustomerPurchasedByFreelancePtIdCountSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByFreelancePtIdCountSpec(Guid freelancePtId) : base(x => x.OrderItems.Any(x => x.FreelancePTPackage!= null && x.FreelancePTPackage.PtId == freelancePtId)
    && x.IsEnabled)
    {
    }

}
