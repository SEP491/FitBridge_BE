using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
{
    public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
    {
        builder.ToTable("WithdrawalRequests");
        builder.Property(e => e.Status).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<WithdrawalRequestStatus>(s));
        builder.Property(e => e.Amount).IsRequired(true);
        builder.Property(e => e.Note).IsRequired(true);
        builder.Property(e => e.BankName).IsRequired(true);
        builder.Property(e => e.AccountName).IsRequired(true);
        builder.Property(e => e.AccountNumber).IsRequired(true);
        builder.Property(e => e.ImageUrl).IsRequired(true);
        builder.Property(e => e.Reason).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.AccountId).IsRequired(true);
        builder.Property(e => e.IsUserApproved).HasDefaultValue(false);

        builder.HasOne(e => e.Account).WithMany(e => e.WithdrawalRequests).HasForeignKey(e => e.AccountId);
    }
}