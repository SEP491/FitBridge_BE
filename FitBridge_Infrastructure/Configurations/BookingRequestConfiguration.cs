using System;
using FitBridge_Domain.Entities.Trainings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class BookingRequestConfiguration : IEntityTypeConfiguration<BookingRequest>
{
    public void Configure(EntityTypeBuilder<BookingRequest> builder)
    {
        builder.ToTable("BookingRequests");
        builder.Property(e => e.CustomerId).IsRequired(true);
        builder.Property(e => e.PtId).IsRequired(true);
        builder.Property(e => e.TargetBookingId).IsRequired(false);
        builder.Property(e => e.CustomerPurchasedId).IsRequired(true);
        builder.Property(e => e.Note).IsRequired(false);
        builder.Property(e => e.StartTime).IsRequired(true);
        builder.Property(e => e.EndTime).IsRequired(true);
        builder.Property(e => e.BookingDate).IsRequired(true);
        builder.Property(e => e.RequestType).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Customer).WithMany(e => e.CustomerBookingRequests).HasForeignKey(e => e.CustomerId);
        builder.HasOne(e => e.Pt).WithMany(e => e.PtBookingRequests).HasForeignKey(e => e.PtId);
        builder.HasOne(e => e.TargetBooking).WithMany(e => e.BookingRequests).HasForeignKey(e => e.TargetBookingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.CustomerPurchased).WithMany(e => e.BookingRequests).HasForeignKey(e => e.CustomerPurchasedId);
    }
}
