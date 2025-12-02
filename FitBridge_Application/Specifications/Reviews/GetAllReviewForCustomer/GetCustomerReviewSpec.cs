using System;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Application.Specifications;
using System.Linq.Expressions;
using FitBridge_Domain.Enums.Reviews;
namespace FitBridge_Application.Specifications.Reviews.GetAllReviewForCustomer;

public class GetCustomerReviewSpec : BaseSpecification<Review>
{
    public GetCustomerReviewSpec(GetCustomerReviewParams parameters)
        : base(x => parameters.CustomerId == null || x.UserId == parameters.CustomerId &&
        (parameters.ReviewType == null || (parameters.ReviewType == ReviewType.GymCourse && x.GymId != null) || (parameters.ReviewType == ReviewType.FreelancePTPackage && x.FreelancePtId != null) || (parameters.ReviewType == ReviewType.ProductDetail && x.ProductDetailId != null)))
    {
        AddInclude(x => x.ProductDetail);
        AddInclude(x => x.ProductDetail.Product);
        AddInclude(x => x.ProductDetail.Flavour);
        AddInclude(x => x.ProductDetail.Weight);
        AddInclude(x => x.User);
        AddInclude(x => x.Gym);
        AddInclude(x => x.FreelancePt);
        AddOrderByDesc(x => x.CreatedAt);
        if (parameters.DoApplyPaging)
        {
            AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
        }
        else
        {
            parameters.Size = -1;
            parameters.Page = -1;
        }
    }
}
