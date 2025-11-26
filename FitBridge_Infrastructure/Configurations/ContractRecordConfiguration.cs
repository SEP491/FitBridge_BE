using System;
using FitBridge_Domain.Entities.Contracts;
using FitBridge_Domain.Enums.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ContractRecordConfiguration : IEntityTypeConfiguration<ContractRecord>
{
    public void Configure(EntityTypeBuilder<ContractRecord> builder)
    {
        builder.Property(e => e.CustomerId).IsRequired();
        builder.Property(e => e.ContractType).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<ContractType>(s));
        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.EndDate).IsRequired();
        builder.Property(e => e.FullName).IsRequired();
        builder.Property(e => e.IdentityCardNumber).IsRequired();
        builder.Property(e => e.IdentityCardDate).IsRequired();
        builder.Property(e => e.IdentityCardPlace).IsRequired();
        builder.Property(e => e.PermanentAddress).IsRequired();
        builder.Property(e => e.PhoneNumber).IsRequired();
        builder.Property(e => e.TaxCode).IsRequired();
        builder.Property(e => e.BusinessAddress).IsRequired(false);
        builder.Property(e => e.CommissionPercentage).IsRequired();
        builder.Property(e => e.ContractUrl).IsRequired(false);
        builder.Property(e => e.CompanySignatureUrl).IsRequired(false);
        builder.Property(e => e.CustomerSignatureUrl).IsRequired(false);
        builder.Property(e => e.ContractStatus).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<ContractStatus>(s));
        builder.Property(e => e.ExtraRules).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Customer).WithMany(e => e.ContractRecords).HasForeignKey(e => e.CustomerId);
    }
}
