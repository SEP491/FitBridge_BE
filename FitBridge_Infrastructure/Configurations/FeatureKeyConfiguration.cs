using System;
using FitBridge_Domain.Entities.ServicePackages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class FeatureKeyConfiguration : IEntityTypeConfiguration<FeatureKey>
{
    public void Configure(EntityTypeBuilder<FeatureKey> builder)
    {
        builder.ToTable("FeatureKeys");
        builder.Property(e => e.FeatureName).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
