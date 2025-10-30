using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FitBridge_Domain.Entities.Trainings;

namespace FitBridge_Infrastructure.Configurations;

public class BodyMeasurementRecordConfiguration : IEntityTypeConfiguration<BodyMeasurementRecord>
{
    public void Configure(EntityTypeBuilder<BodyMeasurementRecord> builder)
    {
        builder.ToTable("BodyMeasurementRecords");
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
        builder.Property(e => e.CustomerPurchasedId).IsRequired();

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.HasOne(e => e.CustomerPurchased).WithMany(e => e.BodyMeasurementRecords).HasForeignKey(e => e.CustomerPurchasedId);
    }
}
