using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Ecommerce;

namespace FitBridge_Domain.Entities.MessageAndReview;

public class Review : BaseEntity
{
    public double Rating { get; set; }
    public string Content { get; set; }
    public bool IsEdited { get; set; }
    public Guid? UserId { get; set; }
    public Guid? GymId { get; set; }
    public Guid? FreelancePtId { get; set; }
    public Guid? ProductDetailId { get; set; }
    public ICollection<string>? ImageUrls { get; set; }
    public ApplicationUser User { get; set; }
    public ApplicationUser Gym { get; set; }
    public ApplicationUser FreelancePt { get; set; }
    public ProductDetail ProductDetail { get; set; }
}
