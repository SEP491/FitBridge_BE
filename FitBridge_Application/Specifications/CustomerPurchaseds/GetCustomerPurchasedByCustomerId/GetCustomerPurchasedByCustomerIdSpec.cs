using System;
using System.Linq.Expressions;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;
using Newtonsoft.Json.Serialization;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetCustomerPurchasedByCustomerId;

public class GetCustomerPurchasedByCustomerIdSpec : BaseSpecification<CustomerPurchased>
{
    public GetCustomerPurchasedByCustomerIdSpec(Guid accountId,
        GetCustomerPurchasedParams parameters,
        bool isGymCourse = true) : base(x =>
            x.CustomerId == accountId && x.IsEnabled
            && x.OrderItems.Any(x => isGymCourse ? x.GymCourseId != null : x.FreelancePTPackageId != null)
            && (!parameters.IsOngoingOnly ||
            (parameters.IsOngoingOnly && x.ExpirationDate > DateOnly.FromDateTime(DateTime.UtcNow)))
        )
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
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
    }
}