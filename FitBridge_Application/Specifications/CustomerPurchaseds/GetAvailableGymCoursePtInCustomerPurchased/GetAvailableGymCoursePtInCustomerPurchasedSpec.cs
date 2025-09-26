using System;
using FitBridge_Application.Specifications;
using FitBridge_Application.Specifications.GymCoursePts.GetPurchasedGymCoursePtForScheduleGetPurchasedGymCoursePtForSchedule;
using FitBridge_Domain.Entities.Gyms;

namespace FitBridge_Application.Specifications.CustomerPurchaseds.GetAvailableGymCoursePtInCustomerPurchased;

public class GetAvailableGymCoursePtInCustomerPurchasedSpec : BaseSpecification<CustomerPurchased>
{
    public GetAvailableGymCoursePtInCustomerPurchasedSpec(GetPurchasedGymCoursePtForScheduleParams parameters, Guid customerId) : base(x => x.IsEnabled
    && x.ExpirationDate >= DateOnly.FromDateTime(DateTime.UtcNow)
    && x.OrderItem.GymCourseId != null
    && x.OrderItem.GymPtId != null
    && x.CustomerId == customerId
    && x.IsEnabled == true)
    {
        if (parameters.DoApplyPaging)
        {
            AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
        AddInclude(x => x.OrderItem.GymCourse);
        AddInclude(x => x.OrderItem.GymPt);
    }
}
