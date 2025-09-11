using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.Property(e => e.Rating).IsRequired(true);
        builder.Property(e => e.Content).IsRequired(true);
        builder.Property(e => e.IsEdited).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(false);
        builder.Property(e => e.GymId).IsRequired(false);
        builder.Property(e => e.FreelancePtId).IsRequired(false);
        builder.Property(e => e.ProductDetailId).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        // User who wrote the review
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Gym being reviewed
        builder.HasOne(r => r.Gym)
            .WithMany(u => u.GymReviews)
            .HasForeignKey(r => r.GymId)
            .OnDelete(DeleteBehavior.Cascade);

        // Freelance PT being reviewed  
        builder.HasOne(r => r.FreelancePt)
            .WithMany(u => u.FreelancePtReviews)
            .HasForeignKey(r => r.FreelancePtId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product being reviewed
        builder.HasOne(r => r.ProductDetail)
            .WithMany(pd => pd.Reviews)
            .HasForeignKey(r => r.ProductDetailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
