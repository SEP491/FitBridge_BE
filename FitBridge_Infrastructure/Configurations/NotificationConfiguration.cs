using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.Property(e => e.Body).IsRequired(true);
        builder.Property(e => e.TemplateId).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Template).WithOne(e => e.InAppNotification).HasForeignKey<Notification>(e => e.TemplateId);
        builder.HasOne(e => e.User).WithMany(e => e.InAppNotifications).HasForeignKey(e => e.UserId);
        
        builder.Property(e => e.ReadAt).HasDefaultValue(null);
        builder.Property(n => n.AdditionalPayload)
                    .HasColumnType("jsonb");
        builder.HasIndex(n => n.AdditionalPayload)
                   .HasMethod("gin");
    }
}