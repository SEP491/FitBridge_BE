using System;
using FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedMyPackage;

public class GetCustomerPurchasedForMyPackageByCustomerIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedForMyPackageByCustomerIdSpec(Guid customerId, GetCustomerPurchasedParams parameters) : base(x => x.CustomerId == customerId
    && x.IsEnabled
    && x.OrderItems.Any(x => x.GymCourseId != null || x.FreelancePTPackageId != null)
    && (!parameters.IsOngoingOnly ||
    (parameters.IsOngoingOnly && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow)))
    )
    {
        AddInclude(x => x.OrderItems);
        AddInclude("OrderItems.GymCourse");
        AddInclude("OrderItems.GymCourse.GymCoursePTs");
        AddInclude("OrderItems.FreelancePTPackage.Pt");
        AddInclude("OrderItems.FreelancePTPackage");
        AddOrderByDesc(x => x.ExpirationDate);
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
