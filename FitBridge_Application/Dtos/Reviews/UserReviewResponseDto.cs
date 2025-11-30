using System;
using FitBridge_Application.Dtos.FreelancePTPackages;
using FitBridge_Application.Dtos.Gym;
using FitBridge_Application.Dtos.GymCourses;
using FitBridge_Application.Dtos.ProductDetails;
using FitBridge_Domain.Enums.Reviews;
namespace FitBridge_Application.Dtos.Reviews;

public class UserReviewResponseDto
{
    public Guid Id { get; set; }
    public double Rating { get; set; }
    public string Content { get; set; }
    public bool IsEdited { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string? UserAvatarUrl { get; set; }
    public List<string>? ImageUrls { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ReviewType ReviewType { get; set; }
    public ProductDetailForReviewDto? ProductDetail { get; set; }
    public GymReviewBriefDto? GymBrief { get; set; }
    public FreelancePtReviewBriefDto? FreelancePtBrief { get; set; }
}
