using System;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Enums.Gyms;
using FitBridge_Domain.Enums.SessionActivities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class AssetMetadataConfiguration : IEntityTypeConfiguration<AssetMetadata>
{
    public void Configure(EntityTypeBuilder<AssetMetadata> builder)
    {
        builder.ToTable("AssetMetadata");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.AssetType).IsRequired(true).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<AssetType>(s));
        builder.Property(e => e.EquipmentCategoryType).IsRequired(true).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<EquipmentCategoryType>(s));
        builder.Property(e => e.Description).IsRequired(true);
        builder.Property(e => e.TargetMuscularGroups).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }

}
