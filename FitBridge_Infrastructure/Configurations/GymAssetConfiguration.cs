using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class GymAssetConfiguration : IEntityTypeConfiguration<GymAsset>
{
    public void Configure(EntityTypeBuilder<GymAsset> builder)
    {
        builder.ToTable("GymAssets");
        builder.Property(e => e.GymOwnerId).IsRequired(true);
        builder.Property(e => e.AssetMetadataId).IsRequired(true);
        builder.Property(e => e.Quantity).IsRequired(true);
        builder.Property(e => e.ImageUrls).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.HasOne(e => e.GymOwner).WithMany(e => e.GymAssets).HasForeignKey(e => e.GymOwnerId);
        builder.HasOne(e => e.AssetMetadata).WithMany(e => e.GymAssets).HasForeignKey(e => e.AssetMetadataId);
    }
}
