using System;
using Microsoft.EntityFrameworkCore;
using FitBridge_Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitBridge_Domain.Enums.ApplicationUser;

namespace FitBridge_Infrastructure.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.FullName).IsRequired().HasMaxLength(255);
        builder.Property(e => e.IsMale).IsRequired();
        builder.Property(e => e.Dob).IsRequired();
        builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Password).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(255);
        builder.Property(e => e.GymName).IsRequired(false).HasMaxLength(255);
        builder.Property(e => e.TaxCode).IsRequired(false).HasMaxLength(255);
        builder.Property(e => e.Longitude).IsRequired(false);
        builder.Property(e => e.Latitude).IsRequired(false);
        builder.Property(e => e.hotResearch).HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.AccountStatus)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<AccountStatus>(s))
        .HasDefaultValue(AccountStatus.Active);
        builder.Property(e => e.GymOwnerId).IsRequired(false);
        builder.Property(e => e.PtMaxCourse).HasDefaultValue(3);
        builder.Property(e => e.MinimumSlot).HasDefaultValue(1);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.HasOne(e => e.GymOwner)
        .WithMany(e => e.GymPTs)
        .HasForeignKey(e => e.GymOwnerId)
        .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.PhoneNumber).IsUnique();
        builder.HasIndex(e => e.TaxCode).IsUnique();
    }
}