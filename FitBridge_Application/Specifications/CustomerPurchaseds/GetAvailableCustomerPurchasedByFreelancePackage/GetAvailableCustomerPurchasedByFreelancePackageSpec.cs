using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetAvailableCustomerPurchasedByFreelancePackage;

public class GetAvailableCustomerPurchasedByFreelancePackageSpec : BaseSpecification<CustomerPurchased>
{
    public GetAvailableCustomerPurchasedByFreelancePackageSpec(Guid freelancePTPackageId) : base(x =>
        x.IsEnabled &&
        x.OrderItems.Any(oi => oi.FreelancePTPackageId != null && oi.FreelancePTPackage!.Id == freelancePTPackageId) &&
        x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow))
    {
    }

}
