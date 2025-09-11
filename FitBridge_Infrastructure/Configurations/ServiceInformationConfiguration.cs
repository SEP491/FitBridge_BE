using System;
using FitBridge_Domain.Entities.ServicePackages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ServiceInformationConfiguration : IEntityTypeConfiguration<ServiceInformation>
{
    public void Configure(EntityTypeBuilder<ServiceInformation> builder)
    {
        builder.ToTable("ServiceInformations"); 
        builder.Property(e => e.ServiceName).IsRequired(true);
        builder.Property(e => e.ServiceCharge).IsRequired(true);
        builder.Property(e => e.MaximumHotResearchSlot).IsRequired(false);
        builder.Property(e => e.AvailableHotResearchSlot).IsRequired(false);
    
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }
}
