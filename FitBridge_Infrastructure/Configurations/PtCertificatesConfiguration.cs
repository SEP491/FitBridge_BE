using System;
using FitBridge_Domain.Entities.Certificates;
using FitBridge_Domain.Enums.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class PtCertificatesConfiguration : IEntityTypeConfiguration<PtCertificates>
{
    public void Configure(EntityTypeBuilder<PtCertificates> builder)
    {
        builder.ToTable("PtCertificates");
        builder.Property(e => e.PtId).IsRequired(true);
        builder.Property(e => e.CertificateMetadataId).IsRequired(true);
        builder.Property(e => e.CertUrl).IsRequired(true);
        builder.Property(e => e.ProvidedDate).IsRequired(true);
        builder.Property(e => e.ExpirationDate).IsRequired(false);
        builder.Property(e => e.CertificateStatus).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<CertificateStatus>(s));
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.Note).IsRequired(false);
        
        builder.HasOne(e => e.Pt).WithMany(e => e.PtCertificates).HasForeignKey(e => e.PtId);
        builder.HasOne(e => e.CertificateMetadata).WithMany(e => e.PtCertificates).HasForeignKey(e => e.CertificateMetadataId);
    }
}
