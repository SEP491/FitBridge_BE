using System;
using FitBridge_Domain.Entities.MessageAndReview;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitBridge_Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        
        // Property configurations
        builder.Property(e => e.IsGroup).IsRequired(true).HasDefaultValue(false);
        builder.Property(e => e.LastMessageId).IsRequired(false);
        builder.Property(e => e.LastMessageContent).IsRequired(false);
        builder.Property(e => e.LastMessageType).IsRequired(false);
        builder.Property(e => e.LastMessageMediaType).IsRequired(false);
        builder.Property(e => e.LastMessageSenderName).IsRequired(false);
        builder.Property(e => e.LastMessageSenderId).IsRequired(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        // Relationship configurations
        builder.HasMany(e => e.Messages)
            .WithOne(e => e.Conversation)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ConversationMembers)
            .WithOne(e => e.Conversation)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
