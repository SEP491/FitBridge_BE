using System;
using Microsoft.EntityFrameworkCore;
using FitBridge_Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        builder.Property(e => e.GymImages).IsRequired(false);
    }
}
