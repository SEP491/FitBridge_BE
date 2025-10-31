using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");
        builder.Property(e => e.Status).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<TransactionStatus>(s));
        builder.Property(e => e.Amount).IsRequired(true);
        builder.Property(e => e.OrderCode).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(true);
        builder.Property(e => e.TransactionType).IsRequired(true);
        builder.Property(e => e.PaymentMethodId).IsRequired(true);
        builder.Property(e => e.WithdrawalRequestId).IsRequired(false);
        builder.Property(e => e.OrderId).IsRequired(false);
        builder.Property(e => e.ProfitAmount).IsRequired(false);
        builder.Property(e => e.OrderItemId).IsRequired(false);
        builder.HasOne(e => e.PaymentMethod).WithMany(e => e.Transactions).HasForeignKey(e => e.PaymentMethodId);
        builder.HasOne(e => e.WithdrawalRequest).WithMany(e => e.Transactions).HasForeignKey(e => e.WithdrawalRequestId);
        builder.HasOne(e => e.Order).WithMany(e => e.Transactions).HasForeignKey(e => e.OrderId);
        builder.HasOne(e => e.OrderItem).WithMany(e => e.Transactions).HasForeignKey(e => e.OrderItemId);
        builder.HasOne(e => e.Wallet).WithMany(e => e.Transactions).HasForeignKey(e => e.WalletId);
        builder.Property(e => e.TransactionType).HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<TransactionType>(s));
    }
}