using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
{
    public void Configure(EntityTypeBuilder<MessageStatus> builder)
    {
        builder.ToTable("MessageStatuses");

        // Ignore inherited Id property from BaseEntity and use composite key
        builder.Ignore(e => e.Id);
        builder.HasKey(e => new { e.MessageId, e.UserId });

        // Property configurations
        builder.Property(e => e.MessageId).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.SentAt).IsRequired(false);
        builder.Property(e => e.DeliveredAt).IsRequired(false);
        builder.Property(e => e.CurrentStatus).IsRequired(true);
        builder.Property(e => e.ReadAt).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        // Relationship configurations
        builder.HasOne(e => e.Message)
            .WithMany(e => e.MessageStatuses)
            .HasForeignKey(e => e.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany(e => e.MessageStatuses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}