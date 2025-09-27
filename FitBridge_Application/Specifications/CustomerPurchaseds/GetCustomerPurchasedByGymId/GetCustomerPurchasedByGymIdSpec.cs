using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByGymId;

public class GetCustomerPurchasedByGymIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByGymIdSpec(Guid GymId) : base(x => x.OrderItems.Any(x => x.GymCourse.GymOwnerId == GymId) && x.IsEnabled && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow))
    {
    }
}
