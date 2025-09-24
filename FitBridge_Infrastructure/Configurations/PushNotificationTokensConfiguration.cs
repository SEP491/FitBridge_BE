using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class PushNotificationTokensConfiguration : IEntityTypeConfiguration<PushNotificationTokens>
{
    public void Configure(EntityTypeBuilder<PushNotificationTokens> builder)
    {
        builder.ToTable("PushNotificationTokens");
        builder.Property(e => e.DeviceToken).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.User).WithMany(e => e.PushNotificationTokens).HasForeignKey(e => e.UserId);
    }
}
