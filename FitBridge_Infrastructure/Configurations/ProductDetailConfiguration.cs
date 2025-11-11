using System;
using FitBridge_Domain.Entities.Ecommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
{
    public void Configure(EntityTypeBuilder<ProductDetail> builder)
    {
        builder.ToTable("ProductDetails");
        builder.Property(e => e.OriginalPrice).IsRequired(true);
        builder.Property(e => e.DisplayPrice).IsRequired(true);
        builder.Property(e => e.ExpirationDate).IsRequired(true);
        builder.Property(e => e.ImageUrl).IsRequired(false);
        builder.Property(e => e.ProductId).IsRequired(true);
        builder.Property(e => e.WeightId).IsRequired(true);
        builder.Property(e => e.FlavourId).IsRequired(true);
        builder.Property(e => e.Quantity).IsRequired(true).HasDefaultValue(0);
        builder.Property(e => e.IsDisplayed).IsRequired(true).HasDefaultValue(false);
        builder.Property(e => e.SoldQuantity).IsRequired(true).HasDefaultValue(0);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.ServingSizeInformation).IsRequired(false);
        builder.Property(e => e.ServingsPerContainerInformation).IsRequired(false);
        builder.Property(e => e.ProteinPerServingGrams).IsRequired(false);
        builder.Property(e => e.CaloriesPerServingKcal).IsRequired(false);
        builder.Property(e => e.BCAAPerServingGrams).IsRequired(false);
        builder.HasIndex(e => new { e.ProductId, e.WeightId, e.FlavourId, e.IsEnabled, e.ExpirationDate }).IsUnique();
        builder.HasOne(e => e.Product).WithMany(e => e.ProductDetails).HasForeignKey(e => e.ProductId);
        builder.HasOne(e => e.Weight).WithMany(e => e.ProductDetails).HasForeignKey(e => e.WeightId);
        builder.HasOne(e => e.Flavour).WithMany(e => e.ProductDetails).HasForeignKey(e => e.FlavourId);
    }
}
