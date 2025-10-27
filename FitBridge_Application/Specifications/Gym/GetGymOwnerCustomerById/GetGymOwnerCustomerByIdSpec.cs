using System;
using FitBridge_Domain.Entities.Identity;

namespace FitBridge_Application.Specifications.Gym.GetGymOwnerCustomerById;

public class GetAllGymOwnerCustomerByIdSpec : BaseSpecification<ApplicationUser>
{
    public GetAllGymOwnerCustomerByIdSpec(Guid customerId, Guid gymOwnerId) : base(x =>
    x.CustomerPurchased.Count > 0
    &&
    x.CustomerPurchased.Any(x => x.OrderItems.Any(o => o.GymCourseId != null && o.GymCourse!.GymOwnerId == gymOwnerId))
    )
    {
        AddInclude(x => x.CustomerPurchased);
        AddInclude("CustomerPurchased.OrderItems");
        AddInclude("CustomerPurchased.OrderItems.GymPt");
        AddInclude("CustomerPurchased.OrderItems.GymCourse");
    }
}
