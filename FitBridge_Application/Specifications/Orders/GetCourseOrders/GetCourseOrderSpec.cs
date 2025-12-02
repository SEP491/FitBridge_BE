using System;
using FitBridge_Application.Specifications;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Orders.GetCourseOrders;

public class GetCourseOrderSpec : BaseSpecification<Order>
{
    public GetCourseOrderSpec(GetCourseOrderParams parameters) : base(x =>
        (parameters.IsFreelancePtCourse ? x.OrderItems.Any(x => x.FreelancePTPackageId != null) : x.OrderItems.Any(x => x.GymCourseId != null)) &&
        (parameters.CustomerId == null || x.AccountId == parameters.CustomerId) &&
        (parameters.FromDate == null || x.CreatedAt >= parameters.FromDate) &&
        (parameters.ToDate == null || x.CreatedAt <= parameters.ToDate) &&
        (parameters.OrderId == null || x.Id == parameters.OrderId)
        && x.Status == OrderStatus.Finished
    )
    {
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.Coupon);
        AddInclude("OrderItems.FreelancePTPackage");
        AddInclude("OrderItems.GymCourse");
        AddInclude("OrderItems.GymPt");
        AddInclude("OrderItems.GymPt.UserDetail");
        AddInclude("OrderItems.FreelancePTPackage.Pt.UserDetail");
        AddInclude("OrderItems.FreelancePTPackage.Pt");        
        if (parameters.SortOrder == "asc")
        {
            AddOrderBy(x => x.CreatedAt);
        }
        else
        {
            AddOrderByDesc(x => x.CreatedAt);
        }

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
