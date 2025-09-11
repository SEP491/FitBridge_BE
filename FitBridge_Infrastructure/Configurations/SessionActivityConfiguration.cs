using System;
using FitBridge_Domain.Entities.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class SessionActivityConfiguration : IEntityTypeConfiguration<SessionActivity>
{
    public void Configure(EntityTypeBuilder<SessionActivity> builder)
    {
        builder.ToTable("SessionActivities");
        builder.Property(e => e.ActivityType).IsRequired(true);
        builder.Property(e => e.ActivityName).IsRequired(true);
        builder.Property(e => e.MuscleGroups).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.BookingId).IsRequired(true);

        builder.HasOne(e => e.Booking).WithMany(e => e.SessionActivities).HasForeignKey(e => e.BookingId);
    }
}
