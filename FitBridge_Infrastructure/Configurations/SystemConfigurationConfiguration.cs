using System;
using FitBridge_Domain.Entities;
using FitBridge_Domain.Entities.Systems;
using FitBridge_Domain.Enums.SystemConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.ToTable("SystemConfigurations");
        builder.Property(e => e.Key).IsRequired();
        builder.Property(e => e.Value).IsRequired();
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.DataType).HasConversion(convertToProviderExpression: s => s.ToString(),
        convertFromProviderExpression: s => Enum.Parse<SystemConfigurationDataType>(s)).IsRequired();
        builder.Property(e => e.UpdatedById).IsRequired();

        builder.HasIndex(e => e.Key).IsUnique();
        builder.HasOne(e => e.UpdatedBy).WithMany(e => e.SystemConfigurations).HasForeignKey(e => e.UpdatedById);
    }
}
