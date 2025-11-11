using System;
using FitBridge_Domain.Entities.Ecommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class WeightConfiguration : IEntityTypeConfiguration<Weight>
{
    public void Configure(EntityTypeBuilder<Weight> builder)
    {
        builder.ToTable("Weights");
        builder.Property(e => e.Value).IsRequired(true);
        builder.Property(e => e.Unit).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
