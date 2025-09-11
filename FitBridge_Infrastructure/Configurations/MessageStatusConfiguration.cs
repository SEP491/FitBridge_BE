using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class MessageStatusConfiguration : IEntityTypeConfiguration<MessageStatus>
{
    public void Configure(EntityTypeBuilder<MessageStatus> builder)
    {
        
        builder.ToTable("MessageStatuses");
        builder.Ignore(e => e.Id);
        builder.HasKey(e => new { e.MessageId, e.UserId });
        builder.Property(e => e.MessageId).IsRequired(true);
        builder.Property(e => e.UserId).IsRequired(true);
        builder.Property(e => e.SentAt).IsRequired(true);
        builder.Property(e => e.DeliveredAt).IsRequired(true);
        builder.Property(e => e.ReadAt).IsRequired(true);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasOne(e => e.Message).WithOne(e => e.MessageStatus).HasForeignKey<MessageStatus>(e => e.MessageId);
        builder.HasOne(e => e.User).WithMany(e => e.MessageStatuses).HasForeignKey(e => e.UserId);
        
    }
}
