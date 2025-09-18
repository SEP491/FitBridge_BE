using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class GymCourseConfiguration : IEntityTypeConfiguration<GymCourse>
{
    public void Configure(EntityTypeBuilder<GymCourse> builder)
    {
        builder.ToTable("GymCourses");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.Price).IsRequired(true);
        builder.Property(e => e.Duration).IsRequired(true);
        builder.Property(e => e.Type).IsRequired(true);
        builder.Property(e => e.GymOwnerId).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.ImageUrl).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.Type).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<TypeCourseEnum>(s))
        .IsRequired(true);

        builder.HasOne(e => e.GymOwner).WithMany(e => e.GymCourses).HasForeignKey(e => e.GymOwnerId);
    }
    
}
