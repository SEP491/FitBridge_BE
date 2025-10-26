
using FitBridge_Domain.Entities.Gyms;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Entities.ServicePackages;
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
        builder.Property(e => e.ProductDetailId).IsRequired(false);
        builder.Property(e => e.ProfitDistributePlannedDate).IsRequired(false);
        builder.Property(e => e.ProfitDistributeActualDate).IsRequired(false);
        builder.HasOne(e => e.Order).WithMany(e => e.OrderItems).HasForeignKey(e => e.OrderId);
        builder.HasOne(e => e.ServiceInformation).WithMany(e => e.OrderItems).HasForeignKey(e => e.ServiceInformationId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.ProductDetail).WithMany(e => e.OrderItems).HasForeignKey(e => e.ProductDetailId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.GymCourse).WithMany(e => e.OrderItems).HasForeignKey(e => e.GymCourseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.GymPt).WithMany(e => e.OrderItems).HasForeignKey(e => e.GymPtId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.FreelancePTPackage).WithMany(e => e.OrderItems).HasForeignKey(e => e.FreelancePTPackageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.CustomerPurchased).WithMany(e => e.OrderItems).HasForeignKey(e => e.CustomerPurchasedId).OnDelete(DeleteBehavior.Restrict);
    }
}
