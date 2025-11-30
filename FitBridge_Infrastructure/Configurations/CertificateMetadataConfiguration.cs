using System;
using FitBridge_Domain.Entities.Certificates;
using FitBridge_Domain.Enums.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class CertificateMetadataConfiguration : IEntityTypeConfiguration<CertificateMetadata>
{
    public void Configure(EntityTypeBuilder<CertificateMetadata> builder)
    {
        builder.ToTable("CertificateMetadata");
        builder.Property(e => e.Specializations).IsRequired(true);
        builder.Property(e => e.CertificateType).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<CertificateType>(s));
        builder.Property(e => e.CertCode).IsRequired(true);
        builder.Property(e => e.CertName).IsRequired(true);
        builder.Property(e => e.ProviderName).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
    }

}
