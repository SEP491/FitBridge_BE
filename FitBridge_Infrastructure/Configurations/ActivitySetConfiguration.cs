using System;
using FitBridge_Domain.Entities.Trainings;
using FitBridge_Domain.Enums.ActivitySets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ActivitySetConfiguration : IEntityTypeConfiguration<ActivitySet>
{
    public void Configure(EntityTypeBuilder<ActivitySet> builder)
    {
        builder.ToTable("ActivitySets");
        builder.Property(e => e.RestTime).IsRequired(false).HasDefaultValue(0.0);
        builder.Property(e => e.NumOfReps).IsRequired(false).HasDefaultValue(0);
        builder.Property(e => e.WeightLifted).IsRequired(false).HasDefaultValue(0.0);
        builder.Property(e => e.PracticeTime).IsRequired(false).HasDefaultValue(0.0);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.PlannedNumOfReps).IsRequired(false).HasDefaultValue(0);
        builder.Property(e => e.PlannedPracticeTime).IsRequired(false).HasDefaultValue(0.0);
        builder.Property(e => e.SessionActivityId).IsRequired(true);
        builder.Property(e => e.IsCompleted).IsRequired(true).HasDefaultValue(false);
        builder.HasOne(e => e.SessionActivity).WithMany(e => e.ActivitySets).HasForeignKey(e => e.SessionActivityId);
    }
}
