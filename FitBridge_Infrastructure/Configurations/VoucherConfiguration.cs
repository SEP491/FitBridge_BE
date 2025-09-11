using System;
using FitBridge_Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> builder)
    {
        builder.ToTable("Vouchers");
        builder.Property(e => e.MaxDiscount).IsRequired(true);
        builder.Property(e => e.Type).IsRequired(true)
        .HasConversion(convertToProviderExpression: s => s.ToString(), convertFromProviderExpression: s => Enum.Parse<VoucherType>(s));
        builder.Property(e => e.DiscountPercent).IsRequired(true);
        builder.Property(e => e.Quantity).IsRequired(true);
        builder.Property(e => e.CreatorId).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Creator).WithMany(e => e.Vouchers).HasForeignKey(e => e.CreatorId);
    }
}
