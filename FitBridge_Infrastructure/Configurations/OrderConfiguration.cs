using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.Property(e => e.Status)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<OrderStatus>(s))
        .IsRequired(true);
        builder.Property(e => e.CheckoutUrl).IsRequired(false);
        builder.Property(e => e.TotalAmount).IsRequired(true);
        builder.Property(e => e.AddressId).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.VoucherId).IsRequired(false);
        builder.Property(e => e.CustomerPurchasedIdToExtend).IsRequired(false);
        builder.Property(e => e.GymCoursePTIdToAssign).IsRequired(false);

        builder.HasOne(e => e.Address).WithMany(e => e.Orders).HasForeignKey(e => e.AddressId);
        builder.HasOne(e => e.Account).WithMany(e => e.Orders).HasForeignKey(e => e.AccountId);
        builder.HasOne(e => e.Voucher).WithMany(e => e.Orders).HasForeignKey(e => e.VoucherId);
        builder.HasOne(e => e.CustomerPurchasedToExtend).WithMany(e => e.OrderThatExtend).HasForeignKey(e => e.CustomerPurchasedIdToExtend);
        builder.HasOne(e => e.GymCoursePTToAssign).WithMany(e => e.Orders).HasForeignKey(e => e.GymCoursePTIdToAssign);
    }
}