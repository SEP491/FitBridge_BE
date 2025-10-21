using System;
using FitBridge_Domain.Entities.Reports;
using FitBridge_Domain.Enums.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ReportCasesConfiguration : IEntityTypeConfiguration<ReportCases>
{
    public void Configure(EntityTypeBuilder<ReportCases> builder)
    {
        builder.ToTable("ReportCases");
        builder.Property(e => e.ReporterId).IsRequired(true);
        builder.Property(e => e.ReportedUserId).IsRequired(false);
        builder.Property(e => e.OrderItemId).IsRequired(true);
        builder.Property(e => e.Title).IsRequired(true).HasMaxLength(255);
        builder.Property(e => e.Description).IsRequired(false).HasMaxLength(1000);
        builder.Property(e => e.ImageUrls).IsRequired(false);
        builder.Property(e => e.Status).IsRequired(true).HasConversion(convertToProviderExpression: s => s.ToString(),
        convertFromProviderExpression: s => Enum.Parse<ReportCaseStatus>(s));
        builder.Property(e => e.Note).IsRequired(false).HasMaxLength(1000);
        builder.Property(e => e.ResolvedAt).IsRequired(false);
        builder.Property(e => e.IsPayoutPaused).IsRequired(true).HasDefaultValue(false);
        builder.Property(e => e.ReportType).IsRequired(true).HasConversion(convertToProviderExpression: s => s.ToString(),
        convertFromProviderExpression: s => Enum.Parse<ReportCaseType>(s));

        builder.HasOne(e => e.Reporter).WithMany(e => e.ReportCasesCreated).HasForeignKey(e => e.ReporterId);
        builder.HasOne(e => e.ReportedUser).WithMany(e => e.ReportCasesReported).HasForeignKey(e => e.ReportedUserId);
        builder.HasOne(e => e.OrderItem).WithMany(e => e.ReportCases).HasForeignKey(e => e.OrderItemId).OnDelete(DeleteBehavior.Restrict);
    }
}
