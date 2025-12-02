using System;
using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Reviews.GetAllReview;

public class GetAllReviewByTargetIdSpec : BaseSpecification<Review>
{
    public GetAllReviewByTargetIdSpec(GetAllReviewQueryParams queryParams, Guid? GymId = null, Guid? FreelancePtId = null, Guid? ProductId = null) : base(x =>
    (GymId == null || x.GymId != null && x.GymId == GymId) &&
    (FreelancePtId == null || x.FreelancePtId != null && x.FreelancePtId == FreelancePtId) &&
    (ProductId == null || x.ProductDetailId != null && x.ProductDetail.ProductId == ProductId)
    && x.IsEnabled)
    {
        AddInclude(x => x.User);
        AddInclude(x => x.Gym);
        AddInclude(x => x.FreelancePt);
        AddInclude(x => x.ProductDetail);
        AddInclude(x => x.ProductDetail.Flavour);
        AddInclude(x => x.ProductDetail.Weight);
        if (queryParams.SortOrder.ToLower() == "asc")
        {
            AddOrderBy(x => x.CreatedAt);
        }
        else
        {
            AddOrderByDesc(x => x.CreatedAt);
        }
        if (queryParams.DoApplyPaging)
        {
            AddPaging(queryParams.Size * (queryParams.Page - 1), queryParams.Size);
        }
        else
        {
            queryParams.Size = -1;
            queryParams.Page = -1;
        }
    }
}
