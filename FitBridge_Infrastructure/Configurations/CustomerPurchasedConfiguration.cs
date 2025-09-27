using System;
using FitBridge_Domain.Entities.Gyms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class CustomerPurchasedConfiguration : IEntityTypeConfiguration<CustomerPurchased>
{
    public void Configure(EntityTypeBuilder<CustomerPurchased> builder)
    {
        builder.ToTable("CustomerPurchased");
        builder.Property(e => e.AvailableSessions).IsRequired();
        builder.Property(e => e.CustomerId).IsRequired();
        builder.Property(e => e.ExpirationDate).IsRequired();

        builder.HasOne(e => e.Customer).WithMany(e => e.CustomerPurchased).HasForeignKey(e => e.CustomerId);
    }
}
