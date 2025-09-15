using System;
using FitBridge_Domain.Entities.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.Property(e => e.CustomerId).IsRequired(true);
        builder.Property(e => e.PTGymSlotId).IsRequired(false);
        builder.Property(e => e.BookingDate).IsRequired(true);
        builder.Property(e => e.PtFreelanceStartTime).IsRequired(false);
        builder.Property(e => e.PtFreelanceEndTime).IsRequired(false);
        builder.Property(e => e.Note).IsRequired(false);
        builder.Property(e => e.NutritionTip).IsRequired(false);
        builder.Property(e => e.SessionStatus).IsRequired(true).HasDefaultValue(SessionStatus.Booked);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Customer).WithMany(e => e.Bookings).HasForeignKey(e => e.CustomerId);
        builder.HasOne(e => e.PTGymSlot).WithOne(e => e.Booking).HasForeignKey<Booking>(e => e.PTGymSlotId);
        builder.HasOne(e => e.Order).WithMany(e => e.Bookings).HasForeignKey(e => e.OrderId);
    }
}
