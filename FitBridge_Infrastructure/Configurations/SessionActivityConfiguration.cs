using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.SessionActivities;
using FitBridge_Domain.Enums.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace FitBridge_Infrastructure.Configurations;

public class SessionActivityConfiguration : IEntityTypeConfiguration<SessionActivity>
{
    public void Configure(EntityTypeBuilder<SessionActivity> builder)
    {
        builder.ToTable("SessionActivities");
        builder.Property(e => e.ActivityType).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<ActivityType>(s));
        builder.Property(e => e.ActivityName).IsRequired(true);

        var muscleEnumConverter = new ValueConverter<MuscleGroupEnum, string>(
            v => v.ToString(),
            v => (MuscleGroupEnum)Enum.Parse(typeof(MuscleGroupEnum), v));

        builder.PrimitiveCollection(e => e.MuscleGroups)
            .IsRequired()
            .ElementType()
            .HasConversion(muscleEnumConverter);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.BookingId).IsRequired(true);

        builder.HasOne(e => e.Booking).WithMany(e => e.SessionActivities).HasForeignKey(e => e.BookingId);
    }
}
