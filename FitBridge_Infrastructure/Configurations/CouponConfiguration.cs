using System;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.Property(e => e.MaxDiscount).IsRequired(true);
        builder.Property(e => e.Type).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<CouponType>(s));
        builder.Property(e => e.DiscountPercent).IsRequired(true);
        builder.Property(e => e.Quantity).IsRequired(true);
        builder.Property(e => e.CreatorId).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.HasIndex(e => new {e.CouponCode, e.IsEnabled}).IsUnique();
        builder.HasOne(e => e.Creator).WithMany(e => e.Coupons).HasForeignKey(e => e.CreatorId);
    }
}