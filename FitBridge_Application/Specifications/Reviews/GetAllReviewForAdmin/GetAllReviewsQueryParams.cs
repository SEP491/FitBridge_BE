using System;

namespace FitBridge_Application.Specifications.Reviews.GetAllReviewForAdmin;

public class GetAllReviewsForAdminQueryParams : BaseParams
{
    public Guid? CustomerId { get; set; }
}
