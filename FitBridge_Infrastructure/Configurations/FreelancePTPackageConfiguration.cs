using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class FreelancePTPackageConfiguration : IEntityTypeConfiguration<FreelancePTPackage>
{
    public void Configure(EntityTypeBuilder<FreelancePTPackage> builder)
    {
        builder.ToTable("PTFreelancePackages");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.Price).IsRequired(true);
        builder.Property(e => e.DurationInDays).IsRequired(true);
        builder.Property(e => e.NumOfSessions).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.ImageUrl).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        
        builder.Property(e => e.PtId).IsRequired(true);
        builder.HasOne(e => e.Pt).WithMany(e => e.PTFreelancePackages).HasForeignKey(e => e.PtId);
    }
}
