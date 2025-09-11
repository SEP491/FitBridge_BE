using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class GymCoursePTConfiguration : IEntityTypeConfiguration<GymCoursePT>
{
    public void Configure(EntityTypeBuilder<GymCoursePT> builder)
    {
        builder.ToTable("GymCoursePTs");
        builder.Property(e => e.GymCourseId).IsRequired(true);
        builder.Property(e => e.PTId).IsRequired(true);
        builder.Property(e => e.Session).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.GymCourse).WithMany(e => e.GymCoursePTs).HasForeignKey(e => e.GymCourseId);
        builder.HasOne(e => e.PT).WithMany(e => e.GymCoursePTs).HasForeignKey(e => e.PTId);
    }
}
