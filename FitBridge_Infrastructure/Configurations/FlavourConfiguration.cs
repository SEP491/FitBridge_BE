using System;
using FitBridge_Domain.Entities.Ecommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class FlavourConfiguration : IEntityTypeConfiguration<Flavour>
{
    public void Configure(EntityTypeBuilder<Flavour> builder)
    {
        builder.ToTable("Flavours");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
