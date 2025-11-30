using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class AssetMetadataConfiguration : IEntityTypeConfiguration<AssetMetadata>
{
    public void Configure(EntityTypeBuilder<AssetMetadata> builder)
    {
        builder.ToTable("AssetMetadata");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.AssetType).IsRequired(true);
        builder.Property(e => e.EquipmentCategoryType).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(true);
        builder.Property(e => e.TargetMuscularGroups).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }

}
