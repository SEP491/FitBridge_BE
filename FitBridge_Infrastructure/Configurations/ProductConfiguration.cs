using System;
using FitBridge_Domain.Entities.Ecommerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.Property(e => e.Name).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(true);
        builder.Property(e => e.CoverImageUrl).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);
        builder.Property(e => e.SubCategoryId).IsRequired(true);
        builder.Property(e => e.BrandId).IsRequired(true);
        builder.Property(e => e.IsDisplayed).IsRequired(true).HasDefaultValue(false);
        builder.Property(e => e.ProteinSources).IsRequired(false);
        builder.Property(e => e.CountryOfOrigin).IsRequired(false);
        builder.HasOne(e => e.SubCategory).WithMany(e => e.Products).HasForeignKey(e => e.SubCategoryId);
        builder.HasOne(e => e.Brand).WithMany(e => e.Products).HasForeignKey(e => e.BrandId);
    }
}
