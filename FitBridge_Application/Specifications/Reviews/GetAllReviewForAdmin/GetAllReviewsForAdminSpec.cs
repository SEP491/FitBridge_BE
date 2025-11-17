using System;
using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Reviews.GetAllReviewForAdmin;

public class GetAllReviewsForAdminSpec : BaseSpecification<Review>
{
    public GetAllReviewsForAdminSpec(GetAllReviewsForAdminQueryParams parameters) : base(x => (parameters.CustomerId == null || x.UserId == parameters.CustomerId)
    &&
    (parameters.SearchTerm == null || x.Content.ToLower().Contains(parameters.SearchTerm.ToLower())))
    {
        if(parameters.SortOrder.ToLower() == "asc")
        {
            AddOrderBy(x => x.CreatedAt);
        }
        else
        {
            AddOrderByDesc(x => x.CreatedAt);
        }
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
