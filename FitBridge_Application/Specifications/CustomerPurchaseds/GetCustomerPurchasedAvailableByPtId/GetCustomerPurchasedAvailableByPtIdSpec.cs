using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedAvailableByPtId;

public class GetCustomerPurchasedAvailableByPtIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedAvailableByPtIdSpec(Guid? ptId, Guid? FreelancePtId) : base(x => x.IsEnabled
    && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow)
    && (ptId == null || x.OrderItems.Any(x => x.GymPtId != null && x.GymPtId == ptId))
    && (FreelancePtId == null || x.OrderItems.Any(x => x.FreelancePTPackage != null && x.FreelancePTPackage.PtId == FreelancePtId))
    )
    {
        AddInclude(x => x.OrderItems);
    }

}
