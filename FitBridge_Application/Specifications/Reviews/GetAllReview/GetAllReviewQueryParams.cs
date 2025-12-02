using System;

namespace FitBridge_Application.Specifications.Reviews.GetAllReview;

public class GetAllReviewQueryParams : BaseParams
{
    public Guid? GymCourseId { get; set; }
    public Guid? FreelancePtCourseId { get; set; }
    public Guid? ProductId { get; set; }
}
