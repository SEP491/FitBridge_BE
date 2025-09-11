using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class GymFacilityConfiguration : IEntityTypeConfiguration<GymFacility>
{
    public void Configure(EntityTypeBuilder<GymFacility> builder)
    {
        builder.ToTable("GymFacilities");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.Quantity).IsRequired(true);
        builder.Property(e => e.ImageUrls).IsRequired(false);
        builder.Property(e => e.GymOwnerId).IsRequired(true);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.GymOwner).WithMany(e => e.GymFacilities).HasForeignKey(e => e.GymOwnerId);
    }
}
