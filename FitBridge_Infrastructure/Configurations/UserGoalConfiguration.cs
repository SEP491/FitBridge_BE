using System;
using FitBridge_Domain.Entities.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
{
    public void Configure(EntityTypeBuilder<UserGoal> builder)
    {
        builder.ToTable("UserGoals");
        builder.Property(e => e.TargetBiceps).IsRequired(false);
        builder.Property(e => e.TargetForeArm).IsRequired(false);
        builder.Property(e => e.TargetThigh).IsRequired(false);
        builder.Property(e => e.TargetCalf).IsRequired(false);
        builder.Property(e => e.TargetChest).IsRequired(false);
        builder.Property(e => e.TargetWaist).IsRequired(false);
        builder.Property(e => e.TargetHip).IsRequired(false);
        builder.Property(e => e.TargetShoulder).IsRequired(false);
        builder.Property(e => e.TargetHeight).IsRequired(false);
        builder.Property(e => e.TargetWeight).IsRequired(false);
    
        builder.Property(e => e.StartBiceps).IsRequired(false);
        builder.Property(e => e.StartForeArm).IsRequired(false);
        builder.Property(e => e.StartThigh).IsRequired(false);
        builder.Property(e => e.StartCalf).IsRequired(false);
        builder.Property(e => e.StartChest).IsRequired(false);
        builder.Property(e => e.StartWaist).IsRequired(false);
        builder.Property(e => e.StartHip).IsRequired(false);
        builder.Property(e => e.StartShoulder).IsRequired(false);
        builder.Property(e => e.StartHeight).IsRequired(false);
        builder.Property(e => e.StartWeight).IsRequired(false);

        builder.Property(e => e.FinalBiceps).IsRequired(false);
        builder.Property(e => e.FinalForeArm).IsRequired(false);
        builder.Property(e => e.FinalThigh).IsRequired(false);
        builder.Property(e => e.FinalCalf).IsRequired(false);
        builder.Property(e => e.FinalChest).IsRequired(false);
        builder.Property(e => e.FinalWaist).IsRequired(false);
        builder.Property(e => e.FinalHip).IsRequired(false);
        builder.Property(e => e.FinalShoulder).IsRequired(false);

        builder.Property(e => e.ImageUrl).IsRequired(false);
        builder.Property(e => e.OrderId).IsRequired(true);

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Order).WithOne(e => e.UserGoal).HasForeignKey<UserGoal>(e => e.OrderId);
    }
}
