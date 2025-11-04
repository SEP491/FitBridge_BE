using System;
using FitBridge_Domain.Entities.ServicePackages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class SubscriptionPlansInformationConfiguration : IEntityTypeConfiguration<SubscriptionPlansInformation>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlansInformation> builder)
    {
        builder.ToTable("SubscriptionPlansInformation");
        builder.Property(e => e.PlanName).IsRequired(true);
        builder.Property(e => e.PlanCharge).IsRequired(true);
        builder.Property(e => e.Duration).IsRequired(true);
        builder.Property(e => e.LimitUsage).IsRequired(false);
        builder.Property(e => e.Description).IsRequired(true);
        builder.Property(e => e.ImageUrl).IsRequired(false);
        
        builder.HasOne(e => e.FeatureKey).WithMany(e => e.SubscriptionPlansInformation).HasForeignKey(e => e.FeatureKeyId);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
