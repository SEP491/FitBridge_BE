using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;
using System.Linq;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByPackageId;

public class GetCustomerPurchasedByPackageIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByPackageIdSpec(Guid packageId) : base(x => x.OrderItems.Any(x => x.FreelancePTPackage != null && x.FreelancePTPackage.Id == packageId)
    && x.IsEnabled)
    {
        AddInclude(x => x.OrderItems);
    }

}
