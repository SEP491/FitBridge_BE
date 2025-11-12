using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        
        // Property configurations
        builder.Property(e => e.Content).IsRequired(true);
        builder.Property(e => e.MessageType).IsRequired(true);
        builder.Property(e => e.MediaType).IsRequired(true);
        builder.Property(e => e.Metadata).IsRequired(false);
        builder.Property(e => e.DeletedAt).IsRequired(false);
        builder.Property(e => e.Reaction).IsRequired(false);
        builder.Property(e => e.ConversationId).IsRequired(true);
        builder.Property(e => e.SenderId).IsRequired(false);
        builder.Property(e => e.ReplyToMessageId).IsRequired(false);
        builder.Property(e => e.BookingRequestId).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        // Relationship configurations
        builder.HasOne(e => e.Conversation)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Sender)
            .WithMany(e => e.Messages)
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.ReplyToMessage)
            .WithMany()
            .HasForeignKey(e => e.ReplyToMessageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.BookingRequest)
            .WithMany()
            .HasForeignKey(e => e.BookingRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.MessageStatuses)
            .WithOne(e => e.Message)
            .HasForeignKey(e => e.MessageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
