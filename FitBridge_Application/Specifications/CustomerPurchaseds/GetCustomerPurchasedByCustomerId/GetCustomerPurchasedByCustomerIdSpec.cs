using System;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;

public class GetCustomerPurchasedByCustomerIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByCustomerIdSpec(Guid accountId, GetCustomerPurchasedParams parameters, bool isGymCourse = true) : base(x => x.CustomerId == accountId && x.IsEnabled
    && x.OrderItems.Any(x => isGymCourse ? x.GymCourseId != null : x.FreelancePTPackageId != null))
    {
        AddInclude(x => x.OrderItems);
        if (isGymCourse)
        {
            AddInclude("OrderItems.GymCourse");
            AddInclude("OrderItems.GymCourse.GymCoursePTs");
        }
        else
        {
            AddInclude("OrderItems.FreelancePTPackage");
        }
        AddOrderByDesc(x => x.ExpirationDate);
        AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
    }
}
