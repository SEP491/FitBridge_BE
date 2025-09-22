using System;
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class PTGymSlotConfiguration : IEntityTypeConfiguration<PTGymSlot>
{
    public void Configure(EntityTypeBuilder<PTGymSlot> builder)
    {
        builder.ToTable("PTGymSlots");
        builder.Property(e => e.PTId).IsRequired(true);
        builder.Property(e => e.GymSlotId).IsRequired(true);
        builder.Property(e => e.Status).IsRequired(true).HasDefaultValue(PTGymSlotStatus.Deactivated);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.PT).WithMany(e => e.PTGymSlots).HasForeignKey(e => e.PTId);
        builder.HasOne(e => e.GymSlot).WithMany(e => e.PTGymSlots).HasForeignKey(e => e.GymSlotId);
        builder.HasOne(e => e.Booking).WithOne(e => e.PTGymSlot).HasForeignKey<Booking>(e => e.PTGymSlotId);
    }
}