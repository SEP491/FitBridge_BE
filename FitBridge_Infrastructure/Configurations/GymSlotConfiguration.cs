using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class GymSlotConfiguration : IEntityTypeConfiguration<GymSlot>
{
    public void Configure(EntityTypeBuilder<GymSlot> builder)
    {
        builder.ToTable("GymSlots");
        builder.Property(e => e.StartTime).IsRequired(true);
        builder.Property(e => e.EndTime).IsRequired(true);
        builder.Property(e => e.Name).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.GymOwnerId).IsRequired(true);
        builder.HasOne(e => e.GymOwner).WithMany(e => e.GymSlots).HasForeignKey(e => e.GymOwnerId);
    }
}
