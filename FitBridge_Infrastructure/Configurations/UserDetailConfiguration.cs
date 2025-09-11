using System;
using FitBridge_Domain.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class UserDetailConfiguration : IEntityTypeConfiguration<UserDetail>
{
    public void Configure(EntityTypeBuilder<UserDetail> builder)
    {
        builder.ToTable("UserDetails");
        builder.Property(e => e.Biceps).IsRequired(false);
        builder.Property(e => e.ForeArm).IsRequired(false);
        builder.Property(e => e.Thigh).IsRequired(false);
        builder.Property(e => e.Calf).IsRequired(false);
        builder.Property(e => e.Chest).IsRequired(false);
        builder.Property(e => e.Waist).IsRequired(false);
        builder.Property(e => e.Hip).IsRequired(false);
        builder.Property(e => e.Shoulder).IsRequired(false);
        builder.Property(e => e.Height).IsRequired(false);
        builder.Property(e => e.Weight).IsRequired(false);
        builder.Property(e => e.Certificates).IsRequired(false);
        builder.Property(e => e.Experience).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.User)
        .WithOne(e => e.UserDetail)
        .HasForeignKey<UserDetail>(e => e.Id)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
