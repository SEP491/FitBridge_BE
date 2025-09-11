using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class InAppNotificationConfiguration : IEntityTypeConfiguration<InAppNotification>
{
    public void Configure(EntityTypeBuilder<InAppNotification> builder)
    {
        builder.ToTable("InAppNotifications");
        builder.Property(e => e.Message).IsRequired(true);
        builder.Property(e => e.TemplateId).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Template).WithOne(e => e.InAppNotification).HasForeignKey<InAppNotification>(e => e.TemplateId);
        builder.HasOne(e => e.User).WithMany(e => e.InAppNotifications).HasForeignKey(e => e.UserId);
    }
}
