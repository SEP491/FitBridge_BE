using System;
using FitBridge_Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class GoalTrainingConfiguration : IEntityTypeConfiguration<GoalTraining>
{
    public void Configure(EntityTypeBuilder<GoalTraining> builder)
    {
        builder.ToTable("GoalTrainings");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.GymOwnerId).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.HasMany(e => e.ApplicationUsers).WithMany(e => e.GoalTrainings)
        .UsingEntity(j => j.ToTable("PTGoalTrainings"));
        builder.HasOne(e => e.GymOwner).WithMany(e => e.GymGoalTrainings).HasForeignKey(e => e.GymOwnerId);
    }
}
