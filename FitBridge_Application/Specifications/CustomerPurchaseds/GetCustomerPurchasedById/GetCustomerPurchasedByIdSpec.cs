using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedById;

public class GetCustomerPurchasedByIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByIdSpec(Guid id,
        bool isIncludeBooking = false,
        bool isIncludeSessionActivities = false,
        bool isIncludeActivitySets = false,
        bool isIncludeUserGoals = false) : base(x => x.Id == id
    && x.IsEnabled)
    {
        AddInclude(x => x.OrderItems);

        if (isIncludeBooking)
        {
            AddInclude("Bookings");
        }
        if (isIncludeSessionActivities)
        {
            AddInclude("Bookings.SessionActivities");
        }
        if (isIncludeActivitySets)
        {
            AddInclude("Bookings.SessionActivities.ActivitySets");
        }
        if (isIncludeUserGoals)
        {
            AddInclude(nameof(CustomerPurchased.UserGoal));
        }
    }
}