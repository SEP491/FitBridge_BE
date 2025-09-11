using System;
using FitBridge_Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.Property(e => e.IsFeedback).HasDefaultValue(false);
        builder.Property(e => e.Quantity).IsRequired(true);
        builder.Property(e => e.Price).IsRequired(true);
        builder.Property(e => e.OrderId).IsRequired(true);
        builder.Property(e => e.ProductDetailId).IsRequired(true);

        builder.HasOne(e => e.Order).WithMany(e => e.OrderItems).HasForeignKey(e => e.OrderId);
        builder.HasOne(e => e.ProductDetail).WithMany(e => e.OrderItems).HasForeignKey(e => e.ProductDetailId);
    }
}
