using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistories");
        builder.Property(e => e.Status)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<OrderStatus>(s))
        .IsRequired(true);
        builder.Property(e => e.PreviousStatus)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<OrderStatus>(s))
        .IsRequired(false);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        
        builder.HasOne(e => e.Order).WithMany(e => e.OrderStatusHistories).HasForeignKey(e => e.OrderId);
    }
}
