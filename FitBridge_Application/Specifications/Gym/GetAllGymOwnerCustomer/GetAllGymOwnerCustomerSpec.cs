using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Enums.ApplicationUser;

namespace FitBridge_Application.Specifications.Gym.GetAllGymOwnerCustomer;

public class GetAllGymOwnerCustomerSpec : BaseSpecification<ApplicationUser>
{
    public GetAllGymOwnerCustomerSpec(GetAllGymOwnerCustomerParams parameters, Guid gymOwnerId) : base(x =>
    x.CustomerPurchased.Count > 0
    &&
    x.CustomerPurchased.Any(x => x.OrderItems.Any(o => o.GymCourseId != null && o.GymCourse!.GymOwnerId == gymOwnerId))
    )
    {
        AddInclude(x => x.CustomerPurchased);
        AddInclude("CustomerPurchased.OrderItems");
        AddInclude("CustomerPurchased.OrderItems.GymPt");
        AddInclude("CustomerPurchased.OrderItems.GymCourse");
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
